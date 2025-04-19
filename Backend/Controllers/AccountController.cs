using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.User;
using IKM_Retro.Models;
using IKM_Retro.Repositories;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController(
        AccountService accountService,
        IOptions<JwtOptions> options,
        UserManager<User> userManager,
        RefreshTokenRepository refreshTokenRepository) : BaseAuthController(options)
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AddUser model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already taken.");
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(errors);
            }

            var token = await refreshTokenRepository.GenerateTokensAsync(user.Id);

            AppendTokenToCookies(token.AccessToken);
            return Ok(token);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUser model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Email and password are required.");
            }
            
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return BadRequest("Invalid email or password.");
            }

            var token = await refreshTokenRepository.GenerateTokensAsync(user.Id);

            AppendTokenToCookies(token.AccessToken);
            return Ok(token);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await accountService.RefreshToken(refreshToken);
            return result;
        }

        [HttpPatch("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] PatchUserProfileBody model)
        {
            try
            {
                var updatedUser = await accountService.UpdateProfileAsync(UserId, model);
                return Ok(new { message = "Profile updated successfully", user = updatedUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            try
            {
                var success = await accountService.ChangePasswordAsync(UserId, model);
                if (success)
                {
                    return Ok(new { message = "Password changed successfully" });
                }
                return BadRequest(new { error = "Failed to change password" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}


// using IKM_Retro.DTOs.User;
// using IKM_Retro.Services.Interfaces;
// using Microsoft.AspNetCore.Mvc;
//
// namespace IKM_Retro.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class AccountController(IAccountService _accountService) : ControllerBase
//     {
//         [HttpPost("register")]
//         public Task<IActionResult> Register([FromBody] AddUser model) => _accountService.Register(model);
//
//         [HttpPost("login")]
//         public Task<IActionResult> Login([FromBody] LoginUser model) => _accountService.Login(model);
//
//         [HttpPost("refresh")]
//         public Task<IActionResult> RefreshToken([FromBody] string refreshToken) => _accountService.RefreshToken(refreshToken);
//     }
// }