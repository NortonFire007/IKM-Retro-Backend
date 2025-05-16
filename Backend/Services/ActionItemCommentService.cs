using IKM_Retro.DTOs.Retrospective.Comments;
using IKM_Retro.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Mapster;

namespace IKM_Retro.Services;

public class ActionItemCommentService(ActionItemCommentRepository repository, ILogger<ActionItemCommentService> logger)
{
    public async Task<BaseActionItemCommentDto> CreateAsync(string userId, PostActionItemCommentRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new action item comment for ActionItem {ActionItemId}", request.ActionItemId);

        ActionItemComment newActionItemComment = new()
        {
            ActionItemId = request.ActionItemId,
            Content = request.Content,
            UserId = userId
        };

        repository.Create(newActionItemComment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);

        return newActionItemComment.Adapt<BaseActionItemCommentDto>();
    }

    public async Task<BaseActionItemCommentDto> UpdateAsync(ActionItemComment actionItemComment, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating action item comment with Id {CommentId}", actionItemComment.Id);

        repository.Update(actionItemComment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);

        return actionItemComment.Adapt<BaseActionItemCommentDto>();
    }

    public async Task DeleteAsync(int commentId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting action item comment with Id {CommentId}", commentId);

        ActionItemComment? comment = await repository.GetByIdAsync(commentId);
        if (comment == null)
        {
            logger.LogWarning("action item comment with Id {CommentId} not found.", commentId);
            throw new NotFoundException("Comment not found");
        }

        repository.Delete(comment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
    }

    public async Task<BaseActionItemCommentDto?> GetByIdAsync(int commentId)
    {
        logger.LogInformation("Getting action item comment by Id {CommentId}", commentId);

        var comment = await repository.GetByIdAsync(commentId);
        return comment?.Adapt<BaseActionItemCommentDto>();
    }

    public async Task<List<BaseActionItemCommentDto>> GetByActionItemIdAsync(int actionItemId)
    {
        logger.LogInformation("Getting comments for ActionItem {ActionItemId}", actionItemId);

        var comments = await repository.GetByActionItemIdAsync(actionItemId);
        return comments.Adapt<List<BaseActionItemCommentDto>>();
    }
}