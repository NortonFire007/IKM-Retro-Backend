using IKM_Retro.DTOs.Retrospective.ActionItem;
using IKM_Retro.Models.Retro;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActionItemController(ActionItemService service) : ControllerBase
{
    [HttpGet("by-retrospective/{retrospectiveId:guid}")]
    public async Task<IActionResult> GetByRetrospective(Guid retrospectiveId, CancellationToken cancellationToken)
    {
        var items = await service.GetByRetrospectiveIdAsync(retrospectiveId, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await service.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActionItemDto dto, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.ActionId }, created);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateActionItemDto patchDto,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, patchDto, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}