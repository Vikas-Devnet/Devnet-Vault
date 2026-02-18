using Application.Features.Common.Interfaces;

namespace Application.Features.Common.Services;

public class OtpService(IRedisCacheService _redis) : IOtpService
{
    public async Task<string> GenerateOtpAsync(string userKey, TimeSpan otpExpireTime, CancellationToken ctx = default)
    {
        string? entity = await _redis.GetAsync<string>(userKey, ctx);
        if (entity != null)
            await _redis.RemoveAsync(userKey, ctx);

        var otp = new Random().Next(100000, 999999).ToString();

        await _redis.SetAsync(userKey, otp, otpExpireTime, otpExpireTime, ctx);

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(string userKey, string otp, CancellationToken ctx = default)
    {
        string? entity = await _redis.GetAsync<string>(userKey, ctx);

        if (entity == null || entity != otp)
            return false;

        await _redis.RemoveAsync(userKey, ctx);
        return true;
    }
}
