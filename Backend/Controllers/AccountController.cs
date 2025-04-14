using IKM_Retro.DTOs.User;
using IKM_Retro.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AddUser model)
        {
            var result = await accountService.Register(model);
            return result;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUser model)
        {
            var result = await accountService.Login(model);
            return result;
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await accountService.RefreshToken(refreshToken);
            return result;
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUser model)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            try
            {
                var updatedUser = await accountService.UpdateProfileAsync(userId, model);
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
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            try
            {
                var success = await accountService.ChangePasswordAsync(userId, model);
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