using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Models.Retro;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InviteController(InviteService inviteService, IOptions<JwtOptions> options) : BaseAuthController(options)
{
    [HttpPost("{retrospectiveId:guid}")]
    public async Task<IActionResult> GenerateInvite(Guid retrospectiveId, CancellationToken cancellationToken)
    {
        RetrospectiveInvite invite = await inviteService.GenerateInviteAsync(retrospectiveId, cancellationToken);
        InviteDto inviteDto = new()
        {
            Code = invite.Code,
            ExpiresAt = invite.ExpiresAt
        };
        return Ok(inviteDto);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetRetrospectiveByInvite(string code, CancellationToken cancellationToken)
    {
        Retrospective? retrospective = await inviteService.GetRetrospectiveByInviteAsync(code, cancellationToken);

        if (retrospective == null)
            return NotFound();

        return Ok(retrospective);
    }
}