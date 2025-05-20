using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.Retrospective.Comments;
using IKM_Retro.Models.Retro;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers;

[ApiController]
[Authorize]
[Route("api/retrospectives/{retrospectiveId:guid}/items/{groupItemId:int}/comments")]
public class GroupItemCommentController(GroupItemCommentService service, IOptions<JwtOptions> options)
    : BaseAuthController(options)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        Guid retrospectiveId,
        int groupItemId,
        [FromBody] PostGroupItemCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(UserId!, groupItemId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{commentId:int}")]
    public async Task<IActionResult> Update(
        Guid retrospectiveId,
        int groupItemId,
        int commentId,
        [FromBody] PostGroupItemCommentRequest groupItemComment,
        CancellationToken cancellationToken)
    {
        var groupItemCommentModel = new GroupItemComment
        {
            Id = commentId,
            GroupItemId = groupItemId,
            Content = groupItemComment.Content,
            IsAnonymous = groupItemComment.IsAnonymous,
            UserId = UserId!
        };

        var result = await service.UpdateAsync(UserId!, retrospectiveId, groupItemCommentModel, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> Delete(
        Guid retrospectiveId,
        int groupItemId,
        int commentId,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(UserId!, retrospectiveId, commentId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{commentId:int}")]
    public async Task<IActionResult> GetById(Guid retrospectiveId, int groupItemId, int commentId)
    {
        var comment = await service.GetByIdAsync(commentId);
        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }

    [HttpGet]
    public async Task<IActionResult> GetByGroupItem(Guid retrospectiveId, int groupItemId)
    {
        var comments = await service.GetByGroupItemIdAsync(groupItemId);
        return Ok(comments);
    }
}