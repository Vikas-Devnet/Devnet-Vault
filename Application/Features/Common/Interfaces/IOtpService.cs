namespace Application.Features.Common.Interfaces;

public interface IOtpService
{
    Task<string> GenerateOtpAsync(string userKey, TimeSpan otpExpireTime, CancellationToken ctx = default);
    Task<bool> VerifyOtpAsync(string userKey, string otp, CancellationToken ctx = default);
}
