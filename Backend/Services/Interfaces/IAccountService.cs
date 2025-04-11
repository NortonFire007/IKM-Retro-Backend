using IKM_Retro.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IActionResult> Register(AddUpdateUser model);
        Task<IActionResult> Login(LoginUser model);
        Task<IActionResult> RefreshToken(string refreshToken);
    }
}
