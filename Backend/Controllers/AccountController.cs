
using IKM_Retro.DTOs.User;
using IKM_Retro.Models;
using IKM_Retro.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController(UserManager<User> _userManager, IRefreshTokenRepository _refreshTokenRepository, IConfiguration _configuration): ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddUpdateUser model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email is already taken");
                    return BadRequest(ModelState);
                }
                var user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await _refreshTokenRepository.GenerateTokensAsync(user.Id);
                    return Ok(token);
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var token = await _refreshTokenRepository.GenerateTokensAsync(user.Id);
                        return Ok(token);
                    }
                }
                ModelState.AddModelError("", "Invalid email or password");
            }
            return BadRequest(ModelState);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetAsync(refreshToken);
            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var newToken = await _refreshTokenRepository.GenerateTokensAsync(user.Id);
            _refreshTokenRepository.Remove(storedToken);

            return Ok(newToken);
        }

    }
}
