using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Enums;
using IKM_Retro.Exceptions.Base;
using IKM_Retro.Hubs;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Mapster;
using Microsoft.AspNetCore.SignalR;

namespace IKM_Retro.Services;

public class GroupItemService(
    GroupItemRepository groupItemRepository,
    RetrospectiveGroupRepository groupRepository,
    RoleService roleService,
    ILogger<GroupItemService> logger,
    ActionItemRepository actionItemRepository,
    IHubContext<GroupItemHub> hubContext)
{
    public async Task<BaseGroupItemDTO> CreateGroupItemAsync(
        string userId,
        PostGroupItemRequest groupItem,
        CancellationToken cancellationToken = default)
    {
        Group group = await groupRepository.GetByIdOr404Async(groupItem.GroupId);
        
        logger.LogInformation("Creating new group item for group {GroupId}", groupItem.GroupId);
        
        var groupItemCount = await groupItemRepository.CountByGroupId(groupItem.GroupId);
        
        GroupItem newGroupItem = new()
        {
            GroupId = groupItem.GroupId,
            Content = groupItem.Content,
            OrderPosition = groupItemCount,
            IsHidden = groupItem.IsHidden,
            UserId = userId
        };
        
        await groupItemRepository.CreateAsync(newGroupItem);
        await groupItemRepository.SaveChangesAsyncWithCancellation(cancellationToken);

        var dto = newGroupItem.Adapt<BaseGroupItemDTO>();

        await hubContext.Clients.Group(group.RetrospectiveId.ToString())
            .SendAsync("ReceiveGroupItemCreated", dto);

        return dto;
    }

    public async Task<BaseGroupItemDTO> PatchGroupItemAsync(
        int groupItemId,
        PatchGroupItemRequest groupItem,
        CancellationToken cancellationToken = default)
    {
        GroupItem existingGroupItem = await groupItemRepository.GetByIdOr404Async(groupItemId);

        Group group = await groupRepository.GetByIdOr404Async(existingGroupItem.GroupId);
        
        if (!string.IsNullOrEmpty(groupItem.Content))
            existingGroupItem.Content = groupItem.Content;
        
        logger.LogInformation("Updating group item with Id {GroupItemId}", groupItemId);
        await groupItemRepository.SaveChangesAsyncWithCancellation(cancellationToken);
        
        var dto = existingGroupItem.Adapt<BaseGroupItemDTO>();

        await hubContext.Clients.Group(group.RetrospectiveId.ToString())
            .SendAsync("ReceiveGroupItemUpdated", dto);

        return dto;
    }

    public async Task<BaseGroupItemDTO> MoveGroupItemAsync(
        int groupItemId,
        int requestedOrderPosition,
        int? newGroupId = null,
        CancellationToken cancellationToken = default)
    {
        GroupItem groupItem = await groupItemRepository.GetByIdOr404Async(groupItemId);

        Group currentGroup = await groupRepository.GetByIdOr404Async(groupItem.GroupId);

        var targetGroupId = newGroupId ?? currentGroup.Id;
        int groupItemCount;
        int newOrderPosition;

        if (targetGroupId == currentGroup.Id)
        {
            groupItemCount = await groupItemRepository.CountByGroupId(currentGroup.Id);
            newOrderPosition = Math.Clamp(requestedOrderPosition, 0, groupItemCount);

            if (newOrderPosition != groupItem.OrderPosition)
            {
                await groupItemRepository.ShiftOrderPositionsWithinGroup(currentGroup.Id, groupItem.OrderPosition, newOrderPosition);
                groupItem.OrderPosition = newOrderPosition;
            }
        }
        else
        {
            Group newGroup = await groupRepository.GetByIdOr404Async(targetGroupId);

            if (currentGroup.RetrospectiveId != newGroup.RetrospectiveId)
                throw new BusinessException("You can't move card to another retrospective");

            groupItemCount = await groupItemRepository.CountByGroupId(targetGroupId);
            newOrderPosition = Math.Clamp(requestedOrderPosition, 0, groupItemCount);

            await groupItemRepository.ShiftOrderPositionsForInsert(targetGroupId, newOrderPosition);
            await groupItemRepository.ShiftOrderPositionsForRemove(currentGroup.Id, groupItem.OrderPosition);

            groupItem.GroupId = targetGroupId;
            groupItem.OrderPosition = newOrderPosition;
        }

        await groupItemRepository.SaveChangesAsyncWithCancellation(cancellationToken);

        var dto = groupItem.Adapt<BaseGroupItemDTO>();

        await hubContext.Clients.Group(currentGroup.RetrospectiveId.ToString())
            .SendAsync("ReceiveGroupItemMoved", dto);

        return dto;
    }
    
    public async Task DeleteGroupItemAsync(
        int groupItemId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        GroupItem existingGroupItem = await groupItemRepository.GetByIdOr404Async(groupItemId);
        Group group = await groupRepository.GetByIdOr404Async(existingGroupItem.GroupId);
        
        var userRole = await roleService.GetUserRoleAsync(userId, group.RetrospectiveId);
        if (userRole == null)
            throw new PermissionException("User does not have access to this retrospective");
        
        if (userRole != RoleTypeEnum.Owner && existingGroupItem.UserId != userId)
            throw new PermissionException("Participants can only delete their own group items");
        
        logger.LogInformation("Deleting group item with Id {GroupItemId}", groupItemId);
        groupItemRepository.DeleteAsync(existingGroupItem);
        await groupItemRepository.SaveChangesAsyncWithCancellation(cancellationToken);
        
        await hubContext.Clients.Group(group.RetrospectiveId.ToString())
            .SendAsync("ReceiveGroupItemDeleted", groupItemId);
    }

    public async Task<List<BaseGroupItemDTO>> GetGroupItemsByRetrospectiveIdAsync(Guid retrospectiveId)
    {
        logger.LogInformation("Getting group items for retrospective {RetrospectiveId}", retrospectiveId);
        var items = await groupItemRepository.GetByRetrospectiveId(retrospectiveId);
        return items.Adapt<List<BaseGroupItemDTO>>();
    }

    public async Task<BaseGroupItemDTO?> GetGroupItemByIdAsync(int groupItemId)
    {
        GroupItem item = await groupItemRepository.GetByIdOr404Async(groupItemId);
        return item.Adapt<BaseGroupItemDTO>();
    }
    
    public async Task<ActionItem> ConvertGroupItemToActionItemAsync(
        int groupItemId,
        ConvertGroupItemToActionItemDto dto,
        CancellationToken cancellationToken = default)
    {
        var groupItem = await groupItemRepository.GetByIdOr404Async(groupItemId);
        var group = await groupRepository.GetByIdOr404Async(groupItem.GroupId);

        var actionItem = new ActionItem
        {
            ActionId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            RetrospectiveId = group.RetrospectiveId,
            Description = groupItem.Content,
            AssignedUserId = dto.AssignedUserId,
            DueDate = null,
            Status = dto.Status ?? ActionItemStatus.Pending,
            Priority = dto.Priority ?? ActionItemPriority.Medium
        };

        await actionItemRepository.AddAsync(actionItem, cancellationToken);
        await actionItemRepository.SaveChangesAsyncWithCancellation(cancellationToken);

        return actionItem;
    }
}