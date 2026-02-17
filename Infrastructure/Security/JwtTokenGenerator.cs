using Application.Features.Account.Interfaces;
using Application.Features.Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtSettingsOption) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtSettingsOption.Value;

    public (string token, DateTime expireTime) GenerateToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
        };
        var expireTime = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expireTime,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expireTime);
    }

    public string GenerateRefreshToken()
    {
        var bytesSpace = new byte[64];
        using var randomBytes = RandomNumberGenerator.Create();
        randomBytes.GetBytes(bytesSpace);
        return Convert.ToBase64String(bytesSpace);
    }
}
