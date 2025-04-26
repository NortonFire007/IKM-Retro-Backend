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
[Route("api/[controller]")]
public class CommentController(CommentService service, IOptions<JwtOptions> options) : BaseAuthController(options)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostCommentRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateCommentAsync(UserId!, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Comment comment, CancellationToken cancellationToken)
    {
        if (id != comment.Id)
        {
            return BadRequest("Mismatched Comment ID");
        }

        var result = await service.UpdateCommentAsync(comment, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteCommentAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await service.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpGet("group-item/{groupItemId}")]
    public async Task<IActionResult> GetByGroupItem(int groupItemId)
    {
        var comments = await service.GetCommentsByGroupItemIdAsync(groupItemId);
        return Ok(comments);
    }
}