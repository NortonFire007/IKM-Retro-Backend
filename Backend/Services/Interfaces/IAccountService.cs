using IKM_Retro.DTOs.User;
using IKM_Retro.Models;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IActionResult> Register(PostUserBody model);
        Task<IActionResult> Login(LoginUser model);
        Task<IActionResult> RefreshToken(string refreshToken);
        Task<User> UpdateProfileAsync(string userId, UpdateUser model);
        Task<bool> ChangePasswordAsync(string userId, ChangePassword model);
    }
}