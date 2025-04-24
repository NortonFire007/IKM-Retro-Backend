using AnimeWebApp.Exceptions.Base;
using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Mapster;

namespace IKM_Retro.Services;

public class GroupItemService(GroupItemRepository repository, ILogger<GroupItemService> logger)
{
    public async Task<BaseGroupItemDTO> CreateGroupItemAsync(string userId, PostGroupItemRequest groupItem,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new group item for group {GroupId}", groupItem.GroupId);
        GroupItem newGroupItem = new()
        {
            GroupId = groupItem.GroupId,
            Content = groupItem.Content,
            OrderPosition = groupItem.OrderPosition,
            IsHidden = groupItem.IsHidden,
            UserId = userId,
        };
        await repository.CreateAsync(newGroupItem);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
        return newGroupItem.Adapt<BaseGroupItemDTO>();
    }


    public async Task<BaseGroupItemDTO> UpdateGroupItemAsync(GroupItem groupItem, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating group item with Id {GroupItemId}", groupItem.Id);
        repository.Update(groupItem);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
        return groupItem.Adapt<BaseGroupItemDTO>();
    }

    public async Task<BaseGroupItemDTO> MoveGroupItemAsync(int groupItemId, int newGroupId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Moving group item {GroupItemId} to group {NewGroupId}", groupItemId, newGroupId);

        var groupItem = await repository.GetByIdAsync(groupItemId);
        if (groupItem == null)
        {
            logger.LogWarning("Group item with Id {GroupItemId} not found.", groupItemId);
            throw new NotFoundException("Group item not found");
        }

        groupItem.GroupId = newGroupId;
        repository.Update(groupItem);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
        return groupItem.Adapt<BaseGroupItemDTO>();
    }

    public async Task DeleteGroupItemAsync(int groupItemId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting group item with Id {GroupItemId}", groupItemId);
        await repository.DeleteAsync(groupItemId);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
    }

    public async Task<List<BaseGroupItemDTO>> GetGroupItemsByRetrospectiveIdAsync(Guid retrospectiveId)
    {
        logger.LogInformation("Getting group items for retrospective {RetrospectiveId}", retrospectiveId);
        var items = await repository.GetByRetrospectiveIdAsync(retrospectiveId);
        return items.Adapt<List<BaseGroupItemDTO>>();
    }

    public async Task<BaseGroupItemDTO?> GetGroupItemByIdAsync(int groupItemId)
    {
        logger.LogInformation("Getting group item by Id {GroupItemId}", groupItemId);
        var item = await repository.GetByIdAsync(groupItemId);
        return item?.Adapt<BaseGroupItemDTO>();
    }
}