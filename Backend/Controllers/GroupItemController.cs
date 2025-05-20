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
[Route("api/retrospectives/{retrospectiveId:guid}/items")]
public class GroupItemController(GroupItemService service, IOptions<JwtOptions> options) : BaseAuthController(options)
{
    [HttpPost]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Create(
        Guid retrospectiveId,
        [FromBody] PostGroupItemRequest request,
        CancellationToken cancellationToken
    )
    {
        BaseGroupItemDTO result = await service.CreateGroupItemAsync(UserId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{itemId:int}")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Update(Guid retrospectiveId, int itemId,
        [FromBody] PatchGroupItemRequest groupItem,
        CancellationToken cancellationToken)
    {
        BaseGroupItemDTO result = await service.PatchGroupItemAsync(itemId, groupItem, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{itemId:int}/move")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Move(Guid retrospectiveId, int itemId, [FromBody] MoveGroupItemRequest request,
        CancellationToken cancellationToken)
    {
        BaseGroupItemDTO result = await service.MoveGroupItemAsync(
            itemId,
            request.OrderPosition,
            request.NewGroupId,
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{itemId:int}")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> Delete(Guid retrospectiveId, int itemId, CancellationToken cancellationToken)
    {
        await service.DeleteGroupItemAsync(itemId, UserId, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> GetByRetrospective(Guid retrospectiveId)
    {
        var items = await service.GetGroupItemsByRetrospectiveIdAsync(retrospectiveId);
        return Ok(items);
    }

    [HttpGet("{itemId:int}")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> GetById(Guid retrospectiveId, int itemId)
    {
        BaseGroupItemDTO? item = await service.GetGroupItemByIdAsync(itemId);
        return Ok(item);
    }

    [HttpPost("{itemId:int}/convert-to-action")]
    [Authorize(Policy = "ParticipantOrOwner")]
    public async Task<IActionResult> ConvertToActionItem(
        Guid retrospectiveId,
        int itemId,
        [FromBody] ConvertGroupItemToActionItemDto dto,
        CancellationToken cancellationToken)
    {
        ActionItem actionItem = await service.ConvertGroupItemToActionItemAsync(itemId, dto, cancellationToken);
        return CreatedAtAction(nameof(ActionItemController.Get), "ActionItem", new { id = actionItem.ActionId },
            actionItem);
    }
}