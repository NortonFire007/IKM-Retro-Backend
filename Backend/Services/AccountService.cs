using AnimeWebApp.Exceptions.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.User;
using IKM_Retro.Models;
using IKM_Retro.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IKM_Retro.Services;

public class AccountService(UserManager<User> userManager, RefreshTokenRepository refreshTokenRepository)
{
    public async Task<User?> GetById(string id)
    {
        return await userManager.FindByIdAsync(id);
    }
    public async Task<JwtToken> Register(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new BusinessException("Email and password are required.");
        }

        User? existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            throw new EntityExistsException("User with that email already exists.");
        }

        User user = new()
        {
            UserName = username,
            Email = email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        IdentityResult result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new BusinessException("An error occured, try again later");

        }

        return await refreshTokenRepository.GenerateTokensAsync(user.Id);

    }

    public async Task<JwtToken> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new BusinessException("Email and password are required.");
        }

        User? user = await userManager.FindByEmailAsync(email);
        if (user == null || !await userManager.CheckPasswordAsync(user, password))
        {
            throw new BusinessException("Invalid email or password.");
        }

        return await refreshTokenRepository.GenerateTokensAsync(user.Id);
    }
    public async Task Logout(string userId)
    {
        User? user = await userManager.FindByIdAsync(userId);

        if (user == null) throw new NotFoundException("User not found");

        RefreshToken? storedRefreshToken = await refreshTokenRepository.GetByUserId(userId);
        if (storedRefreshToken != null)
        {
            refreshTokenRepository.Remove(storedRefreshToken);
            await refreshTokenRepository.SaveChangesAsync();
        }
    }



    public async Task<JwtToken> RefreshToken(string refreshToken)
    {
        RefreshToken? storedToken = await refreshTokenRepository.GetByValue(refreshToken);
        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new BusinessException("Invalid or expired refresh token");
        }

        User user = await userManager.FindByIdAsync(storedToken.UserId) ?? throw new NotFoundException("User not Found");
        JwtToken newToken = await refreshTokenRepository.GenerateTokensAsync(user.Id);
        refreshTokenRepository.Remove(storedToken);

        return newToken;
    }

    public async Task<BaseUserDTO> UpdateProfileAsync(string userId, PatchUserProfileBody model)
    {
        User user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found");

        if (model.Email is not null && !string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            User? existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != userId)
            {
                throw new BusinessException("This email is already in use");
            }

            user.Email = model.Email;
        }

        if (model.UserName is not null && !string.Equals(user.UserName, model.UserName, StringComparison.Ordinal))
        {
            user.UserName = model.UserName;
        }

        IdentityResult result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BusinessException($"Failed to update profile, try again later");
        }

        return new BaseUserDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };

    }


    public async Task ChangePasswordAsync(string userId, ChangePassword model)
    {
        User user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found");
        var passwordCheck = await userManager.CheckPasswordAsync(user, model.CurrentPassword);
        if (!passwordCheck)
        {
            throw new BusinessException("Current password is incorrect");
        }

        IdentityResult result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            throw new BusinessException("Failed to change password");
        }
    }
}