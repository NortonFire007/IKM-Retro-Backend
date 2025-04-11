using IKM_Retro.DTOs.User;
using IKM_Retro.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IKM_Retro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController(IAccountService _accountService) : ControllerBase
    {
        [HttpPost("register")]
        public Task<IActionResult> Register([FromBody] AddUpdateUser model) => _accountService.Register(model);

        [HttpPost("login")]
        public Task<IActionResult> Login([FromBody] LoginUser model) => _accountService.Login(model);

        [HttpPost("refresh")]
        public Task<IActionResult> RefreshToken([FromBody] string refreshToken) => _accountService.RefreshToken(refreshToken);
    }
}
