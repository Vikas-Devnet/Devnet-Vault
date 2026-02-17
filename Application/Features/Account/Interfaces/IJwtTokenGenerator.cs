namespace Application.Features.Account.Interfaces;

public interface IJwtTokenGenerator
{
    (string token, DateTime expireTime) GenerateToken(Guid userId, string email);
    string GenerateRefreshToken();
}
