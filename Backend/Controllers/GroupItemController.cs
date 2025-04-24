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
    public async Task<IActionResult> Create([FromBody] PostGroupItemRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateGroupItemAsync(UserId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GroupItem groupItem, CancellationToken cancellationToken)
    {
        

        var result = await service.UpdateGroupItemAsync(groupItem, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/move")]
    public async Task<IActionResult> Move(int id, [FromQuery] int newGroupId, CancellationToken cancellationToken)
    {
        var result = await service.MoveGroupItemAsync(id, newGroupId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteGroupItemAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("retrospective/{retrospectiveId}")]
    public async Task<IActionResult> GetByRetrospective(Guid retrospectiveId)
    {
        var items = await service.GetGroupItemsByRetrospectiveIdAsync(retrospectiveId);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await service.GetGroupItemByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }
}
