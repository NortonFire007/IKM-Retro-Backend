using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Models.Retro;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RetrospectiveController(RetrospectiveService retrospectiveService, IOptions<JwtOptions> options) : BaseAuthController(options)
    {
        [HttpGet]
        public async Task<List<RetrospectiveToUserDto>> GetByUserId()
        {
            return await retrospectiveService.GetByUserId(UserId);
        }

        // POST api/<RetrospectiveController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostRetrospectiveBody body)
        {
            await retrospectiveService.Create(UserId, body);
            return Ok();
        }
        
        [HttpPost("join/{code}")]
        public async Task<IActionResult> JoinByInvite(string code)
        {
            string userId = User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException();
            await retrospectiveService.JoinByInvite(userId, code);
            return Ok();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await retrospectiveService.Delete(UserId, id);
            return Ok();
        }

    }
}
