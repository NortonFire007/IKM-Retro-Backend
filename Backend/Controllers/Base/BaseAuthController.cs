using IKM_Retro.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers.Base
{
    public class BaseAuthController(IOptions<JwtOptions> options) : ControllerBase
    {
        protected readonly JwtOptions _options = options.Value;
        public string UserId => User.FindFirst("userId")?.Value;
        
    }
}
