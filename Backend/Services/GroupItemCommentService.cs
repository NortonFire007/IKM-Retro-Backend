using IKM_Retro.Exceptions.Base;
using IKM_Retro.DTOs.Retrospective.Comments;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Mapster;

namespace IKM_Retro.Services;

public class GroupItemCommentService(
    GroupItemCommentRepository repository,
    ILogger<GroupItemCommentService> logger,
    RetrospectiveAccessService accessService
)
{
    public async Task<BaseGroupItemCommentDto> CreateAsync(
        string userId,
        int groupItemId,
        PostGroupItemCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new group item comment for GroupItem {GroupItemId}", groupItemId);

        GroupItemComment newGroupItemComment = new()
        {
            GroupItemId = groupItemId,
            Content = request.Content,
            IsAnonymous = request.IsAnonymous,
            UserId = userId,
            Likes = 0
        };

        repository.Create(newGroupItemComment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);

        return newGroupItemComment.Adapt<BaseGroupItemCommentDto>();
    }

    public async Task<PostGroupItemCommentRequest> UpdateAsync(
        string userId,
        Guid retrospectiveId,
        GroupItemComment groupItemComment,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating group item comment with Id {CommentId}", groupItemComment.Id);

        var original = await repository.GetByIdAsync(groupItemComment.Id);
        if (original == null)
            throw new NotFoundException("Comment not found");

        bool isOwner = await accessService.IsOwnerAsync(userId, retrospectiveId);
        if (original.UserId != userId && !isOwner)
            throw new PermissionException("You cannot update someone else's comment");

        original.Content = groupItemComment.Content;
        original.IsAnonymous = groupItemComment.IsAnonymous;

        repository.Update(original);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);

        return original.Adapt<PostGroupItemCommentRequest>();
    }

    public async Task DeleteAsync(
        string userId,
        Guid retrospectiveId,
        int commentId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting group item comment with Id {CommentId}", commentId);

        GroupItemComment? comment = await repository.GetByIdAsync(commentId);
        if (comment == null)
            throw new NotFoundException("Comment not found");

        bool isOwner = await accessService.IsOwnerAsync(userId, retrospectiveId);
        if (comment.UserId != userId && !isOwner)
            throw new PermissionException("You cannot delete someone else's comment");

        repository.Delete(comment);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);
    }


    // public async Task<BaseGroupItemCommentDto> UpdateAsync(GroupItemComment groupItemComment,
    //     CancellationToken cancellationToken = default)
    // {
    //     logger.LogInformation("Updating group item comment with Id {CommentId}", groupItemComment.Id);
    //
    //     repository.Update(groupItemComment);
    //     await repository.SaveChangesAsyncWithCancellation(cancellationToken);
    //
    //     return groupItemComment.Adapt<BaseGroupItemCommentDto>();
    // }

    // public async Task DeleteAsync(int commentId, CancellationToken cancellationToken = default)
    // {
    //     logger.LogInformation("Deleting group item comment with Id {CommentId}", commentId);
    //
    //     GroupItemComment? comment = await repository.GetByIdAsync(commentId);
    //     if (comment == null)
    //     {
    //         logger.LogWarning("Group item comment with Id {CommentId} not found.", commentId);
    //         throw new NotFoundException("Comment not found");
    //     }
    //
    //     repository.Delete(comment);
    //     await repository.SaveChangesAsyncWithCancellation(cancellationToken);
    // }

    public async Task<BaseGroupItemCommentDto?> GetByIdAsync(int commentId)
    {
        logger.LogInformation("Getting group item comment by Id {CommentId}", commentId);

        var comment = await repository.GetByIdAsync(commentId);
        return comment?.Adapt<BaseGroupItemCommentDto>();
    }

    public async Task<List<BaseGroupItemCommentDto>> GetByGroupItemIdAsync(int groupItemId)
    {
        logger.LogInformation("Getting comments for GroupItem {GroupItemId}", groupItemId);

        var comments = await repository.GetByGroupItemIdAsync(groupItemId);
        return comments.Adapt<List<BaseGroupItemCommentDto>>();
    }
}