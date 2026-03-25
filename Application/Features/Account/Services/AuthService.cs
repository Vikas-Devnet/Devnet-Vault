using Application.Features.Account.Dtos;
using Application.Features.Account.Interfaces;
using Application.Features.Common.Interfaces;
using Application.Features.Common.Models;
using Domain.Entities.Account;
using Domain.Interfaces;

namespace Application.Features.Account.Services;

public class AuthService(IUserRepository users, IPasswordHasher hasher, IOtpService _otpService, IEmailQueue _emailQueue, IJwtTokenGenerator jwtTokenGenerator, ICryptoHelper cryptoHelper)
{
    public async Task<ServiceResponseGenerator<bool>> Signup(SignupDto dto, CancellationToken ctx = default)
    {
        var existing = await users.GetByEmailAsync(dto.Email, ctx);
        if (existing != null) return ServiceResponseGenerator<bool>.Failure("Email already Exists");
        var kdfSalt = cryptoHelper.GenerateRandomBytes(16);

        // Master Key (MK)
        var mk = cryptoHelper.DeriveKey(dto.Password, kdfSalt);
        var backupMk = cryptoHelper.DeriveKey(dto.Email, kdfSalt);

        // Data Key (DK)
        var dk = cryptoHelper.GenerateRandomBytes(32);

        // Encrypt DK using MK
        var (encDK, iv, tag) = cryptoHelper.Encrypt(dk, mk);
        var (bk_encDK, bk_iv, bk_tag) = cryptoHelper.Encrypt(dk, backupMk);

        var user = new UserMaster
        {
            Email = dto.Email,
            PasswordHash = hasher.Hash(dto.Password),
            FullName = dto.FullName,
            Nationality = dto.Nationlity,
            CreatedBy = "SignedUp"
        };
        var userKey = new UserKey
        {
            PrimaryEncryptedDK = encDK,
            PrimaryDK_IV = iv,
            PrimaryDK_Tag = tag,
            KdfSalt = kdfSalt,
            BackupEncryptedDK = bk_encDK,
            BackupDK_IV = bk_iv,
            BackupDK_Tag = bk_tag,
            CreatedBy = "SignedUp"
        };

        var response = await users.AddAsync(user, userKey, ctx);
        if (response == null)
            return ServiceResponseGenerator<bool>.Failure("Failed to register user");

        return ServiceResponseGenerator<bool>.Success("User Registered Successfully", true);
    }

    public async Task<ServiceResponseGenerator<AuthResponseDto>> Login(LoginUserDto dto, string ipAddress, CancellationToken ctx = default)
    {
        var user = await users.GetByEmailAsync(dto.Email, ctx);
        if (user == null) return ServiceResponseGenerator<AuthResponseDto>.Failure("Email not valid");

        if (!hasher.Verify(dto.Password, user.PasswordHash))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Invalid password.");

        if (user.IsDeleted)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Account has been deleted");

        if (!user.IsActive)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Account has been deactivated");

        var (token, expireTime) = jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.FullName);
        if (string.IsNullOrEmpty(token))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate security token");
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate refresh security token");

        var refreshTokenObject = new RefreshTokens
        {
            UserId = user.UserId,
            RefreshToken = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(TimeConstants.RefreshTokenExpireTime),
            IPAddress = ipAddress,
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
            CreatedDate = refreshTokenObject.CreatedAt,
            KdfSalt = Convert.ToBase64String(user.UserKey.KdfSalt),
            EncryptedDK = Convert.ToBase64String(user.UserKey.PrimaryEncryptedDK),
            DK_IV = Convert.ToBase64String(user.UserKey.PrimaryDK_IV),
            DK_Tag = Convert.ToBase64String(user.UserKey.PrimaryDK_Tag)
        });
    }

    public async Task<ServiceResponseGenerator<AuthResponseDto>> RefreshAccessToken(string refreshToken, string ipAddress, CancellationToken ctx = default)
    {
        var refreshTokenDetails = await users.GetRefreshTokenValue(refreshToken, ctx);
        if (refreshTokenDetails == null)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Invalid Refresh Token");

        if (refreshTokenDetails.IPAddress != ipAddress)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Invalid IP Address found");

        var userDetails = await users.GetUserByUserIdAsync(refreshTokenDetails.UserId, ctx);

        if (userDetails == null)
            return ServiceResponseGenerator<AuthResponseDto>.Failure("User details not found");

        var (token, expireTime) = jwtTokenGenerator.GenerateToken(refreshTokenDetails.UserId, userDetails.Email, userDetails.FullName);
        if (string.IsNullOrEmpty(token))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate security token");

        var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();
        if (string.IsNullOrEmpty(newRefreshToken))
            return ServiceResponseGenerator<AuthResponseDto>.Failure("Failed to generate refresh security token");

        var refreshTokenObject = new RefreshTokens
        {
            UserId = userDetails.UserId,
            RefreshToken = newRefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(TimeConstants.RefreshTokenExpireTime),
            IPAddress = ipAddress,
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
            CreatedDate = refreshTokenObject.CreatedAt,
            KdfSalt = Convert.ToBase64String(userDetails.UserKey.KdfSalt),
            EncryptedDK = Convert.ToBase64String(userDetails.UserKey.PrimaryEncryptedDK),
            DK_IV = Convert.ToBase64String(userDetails.UserKey.PrimaryDK_IV),
            DK_Tag = Convert.ToBase64String(userDetails.UserKey.PrimaryDK_Tag)
        });

    }

    public async Task<ServiceResponseGenerator<bool>> LogOutUser(bool logoutAllDevices, string ipAddress, string userId, CancellationToken ctx = default)
    {
        if (!Guid.TryParse(userId, out Guid guidUserId))
            return ServiceResponseGenerator<bool>.Failure("Invalid UserId");

        var totalDevicesLoggedOut = await users.InvalidateLogDetails(logoutAllDevices, guidUserId, ipAddress, ctx);
        if (totalDevicesLoggedOut > 0)
            return ServiceResponseGenerator<bool>.Success($"{totalDevicesLoggedOut} Logged out successfully", true);

        return ServiceResponseGenerator<bool>.Failure($"{totalDevicesLoggedOut} devices Logged out");
    }

    public async Task<ServiceResponseGenerator<string>> SendAuthOtp(string email, CancellationToken ctx = default)
    {
        var user = await users.GetByEmailAsync(email, ctx);
        if (user == null) return ServiceResponseGenerator<string>.Failure("Email does not Exists");
        if (user.IsDeleted)
            return ServiceResponseGenerator<string>.Failure("Account has been deleted");

        if (!user.IsActive)
            return ServiceResponseGenerator<string>.Failure("Account has been deactivated");
        var otp = await _otpService.GenerateOtpAsync(user.UserId.ToString(), TimeSpan.FromMinutes(TimeConstants.OtpExpireTime), ctx);

        if (otp == null)
            return ServiceResponseGenerator<string>.Failure("Failed to generate OTP");

        EmailMessage emailMessage = new()
        {
            To = email,
            Subject = "Vault OTP",
            Body = $@"
                Dear {user.FullName},<br><br>
                Your <b>One-Time Password (OTP)</b> for your vault app authentication is:<br><br>
                <h2>{otp}</h2>
                This OTP is valid for <b>{TimeConstants.OtpExpireTime} minutes</b>.<br>
                Please do not share it with anyone.<br><br>
                If you did not request this, please ignore this email.<br><br>
                Regards,<br>
                Support Team
                ",
            IsHtml = true
        };

        bool isAdded = _emailQueue.QueueEmail(emailMessage);
        if (isAdded)
            return ServiceResponseGenerator<string>.Success("Otp has been sent", user.UserId.ToString());

        return ServiceResponseGenerator<string>.Failure("Failed to send Otp. Please try in sometime");

    }

    public async Task<ServiceResponseGenerator<bool>> ValidateAuthOtp(OtpValidationDto dto, CancellationToken ctx = default)
    {
        var isValid = await _otpService.VerifyOtpAsync(dto.OtpKey, dto.Otp, ctx);
        if (isValid)
            return ServiceResponseGenerator<bool>.Success("Otp verified successfully", true);
        return ServiceResponseGenerator<bool>.Failure("Invalid Otp");
    }

    public async Task<ServiceResponseGenerator<bool>> ResetPassword(string email, string currentPasword, string newPassword, string ipAddress, CancellationToken ctx = default)
    {
        var user = await users.GetByEmailAsync(email, ctx);
        if (user == null) return ServiceResponseGenerator<bool>.Failure("Email does not Exists");

        if (!hasher.Verify(currentPasword, user.PasswordHash))
            return ServiceResponseGenerator<bool>.Failure("Invalid current password.");

        if (hasher.Verify(newPassword, user.PasswordHash))
            return ServiceResponseGenerator<bool>.Failure("New password cannot be same as old password");

        if (user.IsDeleted)
            return ServiceResponseGenerator<bool>.Failure("Account has been deleted");

        if (!user.IsActive)
            return ServiceResponseGenerator<bool>.Failure("Account has been deactivated");

        var backupMK = cryptoHelper.DeriveKey(user.Email, user.UserKey.KdfSalt);

        // Decrypt DK
        var dk = cryptoHelper.Decrypt(
            user.UserKey.BackupEncryptedDK,
            backupMK,
            user.UserKey.BackupDK_IV,
            user.UserKey.BackupDK_Tag
        );

        // new password setup
        var newSalt = cryptoHelper.GenerateRandomBytes(16);
        var newMK = cryptoHelper.DeriveKey(newPassword, newSalt);
        var newBackupMK = cryptoHelper.DeriveKey(email, newSalt);

        var (encDK, iv, tag) = cryptoHelper.Encrypt(dk, newMK);
        var (bk_encDK, bk_iv, bk_tag) = cryptoHelper.Encrypt(dk, newBackupMK);

        var userKey = new UserKey
        {
            PrimaryEncryptedDK = encDK,
            PrimaryDK_IV = iv,
            PrimaryDK_Tag = tag,

            BackupEncryptedDK = bk_encDK,
            BackupDK_IV = bk_iv,
            BackupDK_Tag = bk_tag,

            KdfSalt = newSalt,
            UpdatedBy = ipAddress
        };


        var isUpdated = await users.UpdatePassword(userKey, user.UserId, hasher.Hash(newPassword), ipAddress, ctx);
        if (isUpdated)
            return ServiceResponseGenerator<bool>.Success("Password updated successfully", true);
        return ServiceResponseGenerator<bool>.Failure("Password update failed");

    }
}