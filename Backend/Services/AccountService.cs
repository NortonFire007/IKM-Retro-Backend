using IKM_Retro.DTOs.User;
using IKM_Retro.Models;
using IKM_Retro.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Services
{
    public class AccountService(UserManager<User> userManager, RefreshTokenRepository refreshTokenRepository)
    {

        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var storedToken = await refreshTokenRepository.GetAsync(refreshToken);
            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return new UnauthorizedObjectResult(new { message = "Invalid or expired refresh token" });
            }

            var user = await userManager.FindByIdAsync(storedToken.UserId);
            if (user == null) return new UnauthorizedObjectResult(new { message = "User not found" });

            var newToken = await refreshTokenRepository.GenerateTokensAsync(user.Id);
            refreshTokenRepository.Remove(storedToken);

            return new OkObjectResult(newToken);
        }

        public async Task<BaseUserDTO> UpdateProfileAsync(string userId, PatchUserProfileBody model)
        {
            var user = await userManager.FindByIdAsync(userId)
                       ?? throw new KeyNotFoundException("User not found");

            if (model.Email is not null && !string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != userId)
                {
                    throw new InvalidOperationException("This email is already in use");
                }

                user.Email = model.Email;
            }

            if (model.UserName is not null && !string.Equals(user.UserName, model.UserName, StringComparison.Ordinal))
            {
                user.UserName = model.UserName;
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update profile: {errorMessage}");
            }

            return new BaseUserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

        }


        public async Task<bool> ChangePasswordAsync(string userId, ChangePassword model)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var passwordCheck = await userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordCheck)
            {
                throw new InvalidOperationException("Current password is incorrect");
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to change password: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return true;
        }
    }
}