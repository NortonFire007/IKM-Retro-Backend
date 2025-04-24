using IKM_Retro.Controllers.Base;
using IKM_Retro.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IKM_Retro.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RetrospectiveGroupItemController(IOptions<JwtOptions> jwtOptions) : BaseAuthController(jwtOptions)
{

}