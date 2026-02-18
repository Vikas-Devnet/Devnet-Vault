using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<UserMaster?> GetByEmailAsync(string email, CancellationToken ctx = default);
    Task<UserMaster?> GetUserByUserIdAsync(Guid userId, CancellationToken ctx = default);
    Task<UserMaster?> AddAsync(UserMaster user, CancellationToken ctx = default);
    Task<RefreshTokens?> GetRefreshTokenValue(string refreshToken, CancellationToken ctx = default);
    Task<RefreshTokens?> SaveRefreshToken(RefreshTokens refreshTokens, CancellationToken ctx = default);
    Task<RefreshTokens?> UpdateNewRefreshToken(RefreshTokens refreshTokens, string ipAddress, CancellationToken ctx = default);
    Task<int> InvalidateLogDetails(bool logOutAllDevices, Guid userId, string ipAddress, CancellationToken ctx = default);
    Task<bool> UpdatePassword(Guid userId, string password, string ipAddress, CancellationToken ctx = default);
}