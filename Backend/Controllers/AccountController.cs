using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.User;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController(AccountService accountService, IOptions<JwtOptions> options) : BaseAuthController(options)
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] PostUserBody model)
        {
            JwtToken token = await accountService.Register(model.UserName, model.Email, model.Password);
    
            AppendTokenToCookies(token.AccessToken);
            return Ok(token);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUser model)
        {
            JwtToken token = await accountService.Login(model.Email, model.Password);

            AppendTokenToCookies(token.AccessToken);
            return Ok(token);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            return Ok(await accountService.RefreshToken(refreshToken));
        }

        [HttpPatch("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] PatchUserProfileBody model)
        {
            var updatedUser = await accountService.UpdateProfileAsync(UserId, model);
            return Ok(new { message = "Profile updated successfully", user = updatedUser });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            await accountService.ChangePasswordAsync(UserId, model);
            return Ok(new { message = "Password changed successfully" });
        }
    }
}