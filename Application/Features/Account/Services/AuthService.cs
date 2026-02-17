using Application.Features.Account.Dtos;
using Application.Features.Account.Interfaces;
using Application.Features.Common.Interfaces;
using Application.Features.Common.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Features.Account.Services;

public class AuthService(IUserRepository users, IPasswordHasher hasher, IJwtTokenGenerator jwtTokenGenerator, IUtilitiesService utilitiesService)
{
    public async Task<ServiceResponseGenerator<UserMaster>> Signup(SignupDto dto, CancellationToken ctx = default)
    {
        utilitiesService.TrimStrings(dto);
        var existing = await users.GetByEmailAsync(dto.Email, ctx);
        if (existing != null) return ServiceResponseGenerator<UserMaster>.Failure("Email already Exists");

        var user = new UserMaster
        {
            Email = dto.Email,
            PasswordHash = hasher.Hash(dto.Password),
            FullName = dto.FullName,
            Nationality = dto.Nationlity,
            CreatedBy = "SignedUp"
        };

        var response = await users.AddAsync(user, ctx);
        if (response == null)
            return ServiceResponseGenerator<UserMaster>.Failure("Failed to register user");

        return ServiceResponseGenerator<UserMaster>.Success("User Registered Successfully", user);
    }

    public async Task<ServiceResponseGenerator<AuthResponseDto>> Login(LoginUserDto dto, CancellationToken ctx = default)
    {
        utilitiesService.TrimStrings(dto);

        var user = await users.GetByEmailAsync(dto.Email, ctx);
        if (user == null) return ServiceResponseGenerator<AuthResponseDto>.Failure("Email not valid");

        if (!hasher.Verify(dto.Password, user.PasswordHash))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Invalid password.");

        if (user.IsDeleted)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Account has been deleted");

        if (user.IsActive)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Account has been deactivated");

        var (token, expireTime) = jwtTokenGenerator.GenerateToken(user.UserId, user.Email);
        if (string.IsNullOrEmpty(token))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate security token");
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate refresh security token");

        var refreshTokenObject = new RefreshTokens
        {
            UserId = user.UserId,
            RefreshToken = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IPAddress = dto.IPAddress,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "LoggedIn"
        };

        var response = await users.SaveRefreshToken(refreshTokenObject, ctx);
        if (response == null)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to log user");

        return ServiceResponseGenerator<AuthResponseDto>.Success("Credentials Verified Successfully", new AuthResponseDto
        {
            AccessToken = token,
            AccessTokenExpiryDate = expireTime,
            AccessRefreshToken = response.RefreshToken,
            RefreshTokenExpiryDate = refreshTokenObject.ExpiryDate,
            CreatedDate = refreshTokenObject.CreatedAt
        });
    }

    public async Task<ServiceResponseGenerator<AuthResponseDto>> RefreshAccessToken(RefreshTokenDto dto, CancellationToken ctx = default)
    {
        utilitiesService.TrimStrings(dto);

        var refreshTokenDetails = await users.GetRefreshTokenValue(dto.RefreshToken, ctx);
        if (refreshTokenDetails == null)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Invalid Refresh Token");

        if (refreshTokenDetails.IPAddress != dto.IPAddress)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Invalid IP Address found");

        var userDetails = await users.GetUserByUserIdAsync(refreshTokenDetails.UserId, ctx);

        if (userDetails == null)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("User details not found");

        var (token, expireTime) = jwtTokenGenerator.GenerateToken(refreshTokenDetails.UserId, userDetails.Email);
        if (string.IsNullOrEmpty(token))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate security token");

        var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();
        if (string.IsNullOrEmpty(dto.RefreshToken))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate refresh security token");

        var refreshTokenObject = new RefreshTokens
        {
            UserId = userDetails.UserId,
            RefreshToken = newRefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IPAddress = dto.IPAddress,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "RefreshToken"
        };

        var response = await users.SaveRefreshToken(refreshTokenObject, ctx);
        if (response == null)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to refresh token");

        return ServiceResponseGenerator<AuthResponseDto>.Success("Token refreshed successfully", new AuthResponseDto
        {
            AccessToken = token,
            AccessTokenExpiryDate = expireTime,
            AccessRefreshToken = response.RefreshToken,
            RefreshTokenExpiryDate = refreshTokenObject.ExpiryDate,
            CreatedDate = refreshTokenObject.CreatedAt
        });

    }

    public async Task<ServiceResponseGenerator<bool>> LogOutUser(LogoutUserDto dto, string userId, CancellationToken ctx = default)
    {
        utilitiesService.TrimStrings(dto);
        if (!Guid.TryParse(userId, out Guid guidUserId))
            return ServiceResponseGenerator<bool>.Failure("Invalid UserId");

        var totalDevicesLoggedOut = await users.InvalidateLogDetails(dto.LogOutAllDevices, guidUserId, dto.IpAddress, ctx);
        if (totalDevicesLoggedOut > 0)
            return ServiceResponseGenerator<bool>.Success($"{totalDevicesLoggedOut} Logged out successfully", true);

        return ServiceResponseGenerator<bool>.Failure($"{totalDevicesLoggedOut} devices Logged out");
    }
}