using Application.Features.Account.Dtos;
using Application.Features.Account.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(AuthService _auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        var response = await _auth.Login(loginUserDto, HttpContext.RequestAborted);
        if (response.IsSuccess)
        {
            Response.Cookies.Append("AccessToken", response.Result?.AccessToken ?? string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = response.Result?.AccessTokenExpiryDate
            });

            Response.Cookies.Append("RefreshAccessToken", response.Result?.AccessToken ?? string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = response.Result?.RefreshTokenExpiryDate
            });

            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(SignupDto signUpDto)
    {
        var response = await _auth.Signup(signUpDto, HttpContext.RequestAborted);
        if (response.IsSuccess)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("refresh_token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var response = await _auth.RefreshAccessToken(refreshTokenDto, HttpContext.RequestAborted);
        if (response.IsSuccess)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(LogoutUserDto logoutUserDto)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
        var response = await _auth.LogOutUser(logoutUserDto, userIdClaim, HttpContext.RequestAborted);
        if (response.IsSuccess)
        {
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshAccessToken");
            return Ok(response);
        }
        return BadRequest(response);
    }
}
