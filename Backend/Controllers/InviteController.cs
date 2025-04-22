using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InviteController : ControllerBase
{
    private readonly InviteService _inviteService;

    public InviteController(InviteService inviteService)
    {
        _inviteService = inviteService;
    }

    [HttpPost("{retrospectiveId}")]
    public async Task<IActionResult> GenerateInvite(Guid retrospectiveId, CancellationToken cancellationToken)
    {
        var invite = await _inviteService.GenerateInviteAsync(retrospectiveId, cancellationToken);
        var inviteDto = new InviteDto
        {
            Code = invite.Code,
            ExpiresAt = invite.ExpiresAt
        };
        return Ok(inviteDto);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetRetrospectiveByInvite(string code, CancellationToken cancellationToken)
    {
        var retrospective = await _inviteService.GetRetrospectiveByInviteAsync(code, cancellationToken);

        if (retrospective == null)
            return NotFound();

        return Ok(retrospective);
    }
}