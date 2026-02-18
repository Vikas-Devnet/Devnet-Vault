using Application.Features.Account.Dtos;
using Application.Features.Account.Services;
using Application.Features.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(AuthService _auth, IUtilitiesService _utilitiesService) : ControllerBase
{

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        _utilitiesService.TrimStrings(loginUserDto);

        var response = await _auth.Login(loginUserDto, _utilitiesService.ExtractIpAddress(HttpContext), HttpContext.RequestAborted);
        if (response.IsSuccess)
        {
            SetTokenCookies(response.Result ?? throw new Exception("Token Details not found"));
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(SignupDto signUpDto)
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
        return BadRequest(response);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(bool logOutAllDevices)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
        var response = await _auth.LogOutUser(logOutAllDevices, _utilitiesService.ExtractIpAddress(HttpContext), userIdClaim, HttpContext.RequestAborted);
        if (response.IsSuccess)
        {
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshAccessToken");
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet("send-otp")]
    public async Task<IActionResult> SendOtp(string email)
    {
        var response = await _auth.SendAuthOtp(email, HttpContext.RequestAborted);
        if (response.IsSuccess)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(OtpValidationDto otpDto)
    {
        _utilitiesService.TrimStrings(otpDto);
        var response = await _auth.ValidateAuthOtp(otpDto, HttpContext.RequestAborted);
        if (response.IsSuccess)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword(LoginUserDto updatePasswordDto)
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

        Response.Cookies.Append("AccessToken", token.AccessToken, accessTokenOptions);

        Response.Cookies.Append("RefreshAccessToken", token.AccessRefreshToken, refreshTokenOptions);
    }

}
