using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.User;
using IKM_Retro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(AccountService accountService, IOptions<JwtOptions> options) : BaseAuthController(options)
{
    [HttpGet("self")]
    [Authorize]
    public async Task<IActionResult> GetMySelf()
    {
        return Ok(await accountService.GetById(UserId));
    }

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
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await accountService.Logout(UserId);
        RemoveTokenFromCookies();
        return Ok();
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
        BaseUserDTO updatedUser = await accountService.UpdateProfileAsync(UserId, model);
        return Ok(new { message = "Profile updated successfully", user = updatedUser });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
    {
        await accountService.ChangePasswordAsync(UserId, model);
        return Ok(new { message = "Password changed successfully" });
    }

    private void AppendTokenToCookies(string accessToken)
    {
        Response.Cookies.Append(
            _options.CookieName,
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(_options.AccessToken.ExpirationTimeMinutes)
            }
        );
    }
    private void RemoveTokenFromCookies()
    {
        Response.Cookies.Append(
            _options.CookieName,
            "",
            new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            }
        );
    }
    
}