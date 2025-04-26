using IKM_Retro.Exceptions.Base;
using IKM_Retro.DTOs.Retrospective.Comments;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Mapster;

namespace IKM_Retro.Services;

public class CommentService(CommentRepository repository, ILogger<CommentService> logger)
{
    public async Task<BaseCommentDTO> CreateCommentAsync(string userId, PostCommentRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new comment for GroupItem {GroupItemId}", request.GroupItemId);

        Comment newComment = new()
        {
            GroupItemId = request.GroupItemId,
            Content = request.Content,
            IsAnonymous = request.IsAnonymous,
            UserId = userId,
            Likes = 0
        };

        repository.Create(newComment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);

        return newComment.Adapt<BaseCommentDTO>();
    }

    public async Task<BaseCommentDTO> UpdateCommentAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating comment with Id {CommentId}", comment.Id);

        repository.Update(comment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);

        return comment.Adapt<BaseCommentDTO>();
    }

    public async Task DeleteCommentAsync(int commentId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting comment with Id {CommentId}", commentId);

        var comment = await repository.GetByIdAsync(commentId);
        if (comment == null)
        {
            logger.LogWarning("Comment with Id {CommentId} not found.", commentId);
            throw new NotFoundException("Comment not found");
        }

        repository.Delete(comment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
    }

    public async Task<BaseCommentDTO?> GetCommentByIdAsync(int commentId)
    {
        logger.LogInformation("Getting comment by Id {CommentId}", commentId);

        var comment = await repository.GetByIdAsync(commentId);
        return comment?.Adapt<BaseCommentDTO>();
    }

    public async Task<List<BaseCommentDTO>> GetCommentsByGroupItemIdAsync(int groupItemId)
    {
        logger.LogInformation("Getting comments for GroupItem {GroupItemId}", groupItemId);

        var comments = await repository.GetByGroupItemIdAsync(groupItemId);
        return comments.Adapt<List<BaseCommentDTO>>();
    }
}