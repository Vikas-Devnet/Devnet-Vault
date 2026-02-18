using Application.Features.Account.Dtos;
using Application.Features.Account.Services;
using Application.Features.Common.Interfaces;
using Application.Features.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(AuthService _auth, IUtilitiesService _utilitiesService) : ControllerBase
{

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        _utilitiesService.TrimStrings(loginUserDto);

        var response = await _auth.Login(loginUserDto, _utilitiesService.ExtractIpAddress(HttpContext), HttpContext.RequestAborted);
        if (response.IsSuccess)
        {
            SetTokenCookies(response.Result ?? throw new Exception("Token Details not found"));
            return Ok(response);
        }

        return Unauthorized(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SignupDto signUpDto)
    {
        _utilitiesService.TrimStrings(signUpDto);

        var response = await _auth.Signup(signUpDto, HttpContext.RequestAborted);
        if (response.IsSuccess)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = HttpContext.Request.Cookies["RefreshAccessToken"]?.ToString()?.Trim() ?? string.Empty;
        var response = await _auth.RefreshAccessToken(refreshToken, _utilitiesService.ExtractIpAddress(HttpContext), HttpContext.RequestAborted);
        if (response.IsSuccess)
        {
            SetTokenCookies(response.Result ?? throw new Exception("Token Details not found"));
            return Ok(response);
        }
        return Unauthorized(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromQuery] bool logOutAllDevices)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var response = await _auth.LogOutUser(logOutAllDevices, _utilitiesService.ExtractIpAddress(HttpContext), userIdClaim, HttpContext.RequestAborted);
        if (response.IsSuccess)
        {
            Response.Cookies.Delete(CookieConstants.RefreshToken);
            Response.Cookies.Delete(CookieConstants.RefreshToken);
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet("send-otp")]
    public async Task<IActionResult> SendOtp([FromQuery] string email)
    {
        var response = await _auth.SendAuthOtp(email, HttpContext.RequestAborted);
        if (response.IsSuccess)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpValidationDto otpDto)
    {
        _utilitiesService.TrimStrings(otpDto);
        var response = await _auth.ValidateAuthOtp(otpDto, HttpContext.RequestAborted);
        if (response.IsSuccess)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] LoginUserDto updatePasswordDto)
    {
        _utilitiesService.TrimStrings(updatePasswordDto);
        var response = await _auth.ResetPassword(updatePasswordDto.Email, updatePasswordDto.Password,
            _utilitiesService.ExtractIpAddress(HttpContext), HttpContext.RequestAborted);

        if (response.IsSuccess)
            return Ok(response);

        return BadRequest(response);
    }

    private void SetTokenCookies(AuthResponseDto token)
    {
        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = token.AccessTokenExpiryDate,
            IsEssential = true,
            Path = "/"
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = token.RefreshTokenExpiryDate,
            IsEssential = true,
            Path = "/"
        };

        Response.Cookies.Append(CookieConstants.AccessToken, token.AccessToken, accessTokenOptions);

        Response.Cookies.Append(CookieConstants.RefreshToken, token.AccessRefreshToken, refreshTokenOptions);
    }

}
