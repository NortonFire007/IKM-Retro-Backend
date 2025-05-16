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
public class ActionItemCommentController(ActionItemCommentService service, IOptions<JwtOptions> options) : BaseAuthController(options)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostActionItemCommentRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(UserId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActionItemComment actionItemComment, CancellationToken cancellationToken)
    {

        var result = await service.UpdateAsync(actionItemComment, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await service.GetByIdAsync(id);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpGet("action-item/{actionItemId:int}")]
    public async Task<IActionResult> GetByActionItem(int actionItemId)
    {
        var comments = await service.GetByActionItemIdAsync(actionItemId);
        return Ok(comments);
    }
}