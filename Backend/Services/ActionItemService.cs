using IKM_Retro.DTOs.Retrospective.ActionItem;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;

namespace IKM_Retro.Services;

public class ActionItemService(ActionItemRepository repository)
{
    public async Task<List<ActionItem>> GetByRetrospectiveIdAsync(Guid retrospectiveId, CancellationToken cancellationToken)
    {
        return await repository.GetByRetrospectiveIdAsync(retrospectiveId, cancellationToken);
    }

    public async Task<ActionItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<ActionItem> CreateAsync(CreateActionItemDto dto, CancellationToken cancellationToken)
    {
        var item = new ActionItem
        {
            ActionId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            RetrospectiveId = dto.RetrospectiveId,
            Description = dto.Description,
            AssignedUserId = dto.AssignedUserId,
            DueDate = dto.DueDate,
            Status = dto.Status,
            Priority = dto.Priority
        };
        
        await repository.AddAsync(item, cancellationToken);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
        return item;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateActionItemDto patchDto, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null) return false;

        if (patchDto.Description != null) existing.Description = patchDto.Description;
        if (patchDto.Priority.HasValue) existing.Priority = patchDto.Priority.Value;
        if (patchDto.Status.HasValue) existing.Status = patchDto.Status.Value;
        if (patchDto.DueDate.HasValue) existing.DueDate = patchDto.DueDate.Value;
        if (patchDto.AssignedUserId.HasValue) existing.AssignedUserId = patchDto.AssignedUserId.Value;

        repository.Update(existing);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
        return true;
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;

        repository.Remove(existing);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
        return true;
    }
}