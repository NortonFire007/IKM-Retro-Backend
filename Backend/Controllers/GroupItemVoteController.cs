using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupItemVoteController(GroupItemVoteService service, IOptions<JwtOptions> options) : BaseAuthController(options)
{
    [HttpPost]
    public async Task<GroupItemVoteDTO> AddVote([FromBody] PostItemVoteRequest body)
    {
        GroupItemVoteDTO groupItemVote = await service.AddVote(UserId, body.GroupItemId);

        return groupItemVote;
    }

    [HttpDelete("{id:int}")]
    public async Task RemoveVote(int id)
    {
        await service.RemoveVote(UserId, id);
    }
}