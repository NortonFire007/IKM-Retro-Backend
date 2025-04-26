using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Mapster;

namespace IKM_Retro.Services;

public class GroupItemService(
    GroupItemRepository groupItemRepository,
    RetrospectiveGroupRepository groupRepository,
    RetrospectiveRepository retrospectiveRepository,
    ILogger<GroupItemService> logger)
{
    public async Task<BaseGroupItemDTO> CreateGroupItemAsync(
        string userId,
        PostGroupItemRequest groupItem,
        CancellationToken cancellationToken = default)
    {
        Group group = await groupRepository.GetByIdOr404Async(groupItem.GroupId);

        await IsUserInRetrospectiveOrPermissionException(userId, group.RetrospectiveId);
        
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
        return newGroupItem.Adapt<BaseGroupItemDTO>();
    }

    public async Task<BaseGroupItemDTO> PatchGroupItemAsync(
        string userId,
        int groupItemId,
        PatchGroupItemRequest groupItem,
        CancellationToken cancellationToken = default)
    {
        GroupItem existingGroupItem = await groupItemRepository.GetByIdOr404Async(groupItemId);

        Group group = await groupRepository.GetByIdOr404Async(existingGroupItem.GroupId);

        await IsUserInRetrospectiveOrPermissionException(userId, group.RetrospectiveId);
        
        if (!string.IsNullOrEmpty(groupItem.Content))
            existingGroupItem.Content = groupItem.Content;
        
        logger.LogInformation("Updating group item with Id {GroupItemId}", groupItemId);
        await groupItemRepository.SaveChangesAsyncWithCancellation(cancellationToken);
        return existingGroupItem.Adapt<BaseGroupItemDTO>();
    }

    public async Task<BaseGroupItemDTO> MoveGroupItemAsync(
        string userId,
        int groupItemId,
        int requestedOrderPosition,
        int? newGroupId = null,
        CancellationToken cancellationToken = default)
    {
        GroupItem groupItem = await groupItemRepository.GetByIdOr404Async(groupItemId);

        Group currentGroup = await groupRepository.GetByIdOr404Async(groupItem.GroupId);
        await IsUserInRetrospectiveOrPermissionException(userId, currentGroup.RetrospectiveId);

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

        return groupItem.Adapt<BaseGroupItemDTO>();
    }
    
    public async Task DeleteGroupItemAsync(
        string userId,
        int groupItemId,
        CancellationToken cancellationToken = default)
    {
        GroupItem existingGroupItem = await groupItemRepository.GetByIdOr404Async(groupItemId);
        
        Group group = await groupRepository.GetByIdOr404Async(existingGroupItem.GroupId);

        await IsUserInRetrospectiveOrPermissionException(userId, group.RetrospectiveId);
        
        logger.LogInformation("Deleting group item with Id {GroupItemId}", groupItemId);
        groupItemRepository.DeleteAsync(existingGroupItem);
        await groupItemRepository.SaveChangesAsyncWithCancellation(cancellationToken);
    }

    public async Task<List<BaseGroupItemDTO>> GetGroupItemsByRetrospectiveIdAsync(string userId, Guid retrospectiveId)
    {
        if (!await retrospectiveRepository.IsUserInRetrospective(retrospectiveId, userId))
        {
            logger.LogWarning("User {UserId} is not allowed to access retrospective {RetrospectiveId}", userId, retrospectiveId);
            throw new PermissionException("You are not allowed to create or update group items in this retrospective");
        }
        
        logger.LogInformation("Getting group items for retrospective {RetrospectiveId}", retrospectiveId);
        var items = await groupItemRepository.GetByRetrospectiveId(retrospectiveId);
        return items.Adapt<List<BaseGroupItemDTO>>();
    }

    public async Task<BaseGroupItemDTO?> GetGroupItemByIdAsync(int groupItemId)
    {
        GroupItem item = await groupItemRepository.GetByIdOr404Async(groupItemId);
        return item.Adapt<BaseGroupItemDTO>();
    }
    
    private async Task<bool> IsUserInRetrospectiveOrPermissionException(string userId, Guid retrospectiveId)
    {   
        if (await retrospectiveRepository.IsUserInRetrospective(retrospectiveId, userId)) return true;
        logger.LogWarning("User {UserId} is not allowed to access retrospective {RetrospectiveId}", userId, retrospectiveId);
        throw new PermissionException("You are not allowed to create or update group items in this retrospective");
    }
}