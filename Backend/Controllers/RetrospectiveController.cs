using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RetrospectiveController(RetrospectiveService retrospectiveService, IOptions<JwtOptions> options)
    : BaseAuthController(options)
{
    [HttpGet]
    public async Task<List<RetrospectiveToUserDto>> GetByUserId()
    {
        return await retrospectiveService.GetByUserId(UserId);
    }

    [HttpGet("{retrospectiveId:guid}")]
    public async Task<RetrospectiveToUserDto> GetById(Guid retrospectiveId)
    {
        return await retrospectiveService.GetById(retrospectiveId);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostRetrospectiveBody body)
    {
        await retrospectiveService.Create(UserId, body);
        return Ok();
    }

    [HttpPost("join/{code}")]
    public async Task<IActionResult> JoinByInvite(string code)
    {
        await retrospectiveService.JoinByInvite(UserId, code);
        return Ok();
    }

    [HttpDelete("{retrospectiveId:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    public async Task<IActionResult> Delete(Guid retrospectiveId)
    {
        await retrospectiveService.Delete(UserId, retrospectiveId);
        return Ok();
    }

    [HttpPatch("{retrospectiveId:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    public async Task<IActionResult> Update(Guid retrospectiveId, [FromBody] UpdateRetrospectiveDto dto)
    {
        await retrospectiveService.Update(UserId, retrospectiveId, dto);
        return Ok();
    }

    [HttpGet("created")]
    [Authorize]
    public async Task<IActionResult> GetCreated()
    {
        Console.WriteLine($"🟢 UserId from token: {UserId}");

        if (string.IsNullOrEmpty(UserId))
            return Unauthorized("UserId not found in token.");

        var retros = await retrospectiveService.GetCreatedRetrospectivesAsync(UserId);
        return Ok(retros);
    }

    [HttpGet("joined")]
    [Authorize]
    public async Task<IActionResult> GetJoined()
    {
        var retros = await retrospectiveService.GetJoinedRetrospectivesAsync(UserId);
        return Ok(retros);
    }
}