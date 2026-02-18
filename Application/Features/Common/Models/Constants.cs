namespace Application.Features.Common.Models;

public static class CookieConstants
{
    public const string AccessToken = "AccessToken";
    public const string RefreshToken = "RefreshAccessToken";
}
public static class TimeConstants
{
    public const int AccessTokenExpireTime = 10;
    public const int RefreshTokenExpireTime = 7;
    public const int OtpExpireTime = 5;
}
