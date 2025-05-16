using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Models.Retro;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupItemController(GroupItemService service, IOptions<JwtOptions> options) : BaseAuthController(options)
{
    [HttpPost]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Create([FromBody] PostGroupItemRequest request, CancellationToken cancellationToken)
    {
        BaseGroupItemDTO result = await service.CreateGroupItemAsync(UserId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Update(int id, [FromBody] PatchGroupItemRequest groupItem, CancellationToken cancellationToken)
    {
        BaseGroupItemDTO result = await service.PatchGroupItemAsync(id, groupItem, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:int}/move")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Move(int id, [FromBody] MoveGroupItemRequest request , CancellationToken cancellationToken)
    {
        BaseGroupItemDTO result = await service.MoveGroupItemAsync(
            id,
            request.OrderPosition,
            request.NewGroupId,
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteGroupItemAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("retrospective/{retrospectiveId:guid}")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> GetByRetrospective(Guid retrospectiveId)
    {
        var items = await service.GetGroupItemsByRetrospectiveIdAsync(retrospectiveId);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> GetById(int id)
    {
        BaseGroupItemDTO? item = await service.GetGroupItemByIdAsync(id);

        return Ok(item);
    }
    
    [HttpPost("{id:int}/convert-to-action")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> ConvertToActionItem(
        int id,
        [FromBody] ConvertGroupItemToActionItemDto dto,
        CancellationToken cancellationToken)
    {
        ActionItem actionItem = await service.ConvertGroupItemToActionItemAsync(id, dto, cancellationToken);
        return CreatedAtAction(nameof(ActionItemController.Get), "ActionItem", new { id = actionItem.ActionId }, actionItem);
    }

}
