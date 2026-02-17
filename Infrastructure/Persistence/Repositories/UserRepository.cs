using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<UserMaster?> GetByEmailAsync(string email, CancellationToken ctx = default)
        => db.UserMaster.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ctx);

    public Task<RefreshTokens?> GetRefreshTokenValue(string refreshToken, CancellationToken ctx = default)
        => db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(r => r.RefreshToken == refreshToken && r.IsRevoked == false && r.ExpiryDate < DateTime.UtcNow, ctx);

    public Task<UserMaster?> GetUserByUserIdAsync(Guid userId, CancellationToken ctx = default)
       => db.UserMaster.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && x.IsDeleted == false && x.IsActive, ctx);

    public async Task<UserMaster?> AddAsync(UserMaster user, CancellationToken ctx = default)
    {
        try
        {
            db.UserMaster.Add(user);
            var changes = await db.SaveChangesAsync(ctx);
            return changes > 0 ? user : null;
        }
        catch (Exception)
        {
            db.ChangeTracker.Clear();
            throw;
        }
    }

    public async Task<RefreshTokens?> SaveRefreshToken(RefreshTokens refreshTokens, CancellationToken ctx = default)
    {
        try
        {
            db.RefreshTokens.Add(refreshTokens);
            var changes = await db.SaveChangesAsync(ctx);
            return changes > 0 ? refreshTokens : null;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<RefreshTokens?> UpdateNewRefreshToken(RefreshTokens refreshTokens, string ipAddress, CancellationToken ctx = default)
    {
        using var trans = db.Database.BeginTransaction();
        try
        {
            await db.RefreshTokens.Where(re => re.RefreshToken == refreshTokens.RefreshToken && re.IPAddress == ipAddress)
                .ExecuteUpdateAsync(r => r.SetProperty(x => x.IsRevoked, true)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                .SetProperty(x => x.UpdatedBy, ipAddress), ctx);

            db.RefreshTokens.Add(refreshTokens);
            var changes = await db.SaveChangesAsync(ctx);
            if (changes > 0)
            {
                await trans.CommitAsync(ctx);
                return refreshTokens;
            }
            trans.Rollback();

            return null;
        }
        catch (Exception)
        {
            db.ChangeTracker.Clear();
            trans.Rollback();
            throw;
        }
    }

    public async Task<int> InvalidateLogDetails(bool logOutAllDevices, Guid userId, string ipAddress, CancellationToken ctx = default)
    {
        try
        {
            IQueryable<RefreshTokens> query;

            if (logOutAllDevices)
                query = db.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow);
            else
                query = db.RefreshTokens.Where(x => x.UserId == userId && x.IPAddress == ipAddress && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow);

            var changes = await query.ExecuteUpdateAsync(r => r.SetProperty(x => x.IsRevoked, true)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                    .SetProperty(x => x.UpdatedBy, ipAddress), ctx);

            return changes;

        }
        catch (Exception)
        {
            db.ChangeTracker.Clear();
            throw;
        }
    }

}
