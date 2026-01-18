using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Devnet_Vault.Controllers.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService _auth) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var result = await _auth.Login(dto);
            if (!result.IsSuccess) return Unauthorized(result.Error);
            return Ok(new { token = result.Token });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            var result = await _auth.Signup(dto);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(new { token = result.Token });
        }

    }

}
