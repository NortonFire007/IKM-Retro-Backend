using IKM_Retro.DTOs.User;
using IKM_Retro.Models;
using IKM_Retro.Repositories.Interfaces;
using IKM_Retro.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Services
{
    public class AccountService(UserManager<User> userManager, IRefreshTokenRepository refreshTokenRepository) : IAccountService
    {
        public async Task<IActionResult> Register(AddUpdateUser model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return new BadRequestObjectResult("Email and password are required.");
            }

            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new BadRequestObjectResult("Email is already taken.");
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
                return new BadRequestObjectResult(errors);
            }

            var token = await refreshTokenRepository.GenerateTokensAsync(user.Id);
            return new OkObjectResult(token);
        }

        public async Task<IActionResult> Login(LoginUser model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return new BadRequestObjectResult("Email and password are required.");
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return new BadRequestObjectResult("Invalid email or password.");
            }

            var token = await refreshTokenRepository.GenerateTokensAsync(user.Id);
            return new OkObjectResult(token);
        }

        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var storedToken = await refreshTokenRepository.GetAsync(refreshToken);
            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return new UnauthorizedObjectResult(new { message = "Invalid or expired refresh token" });
            }

            var user = await userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return new UnauthorizedObjectResult(new { message = "User not found" });
            }

            var newToken = await refreshTokenRepository.GenerateTokensAsync(user.Id);
            refreshTokenRepository.Remove(storedToken);

            return new OkObjectResult(newToken);
        }
    }
}
