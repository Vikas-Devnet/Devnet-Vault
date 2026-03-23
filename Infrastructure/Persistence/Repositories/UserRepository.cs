using Domain.Entities.Account;
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
        using var trans = db.Database.BeginTransaction();
        try
        {
            db.UserMaster.Add(user);
            var changes = await db.SaveChangesAsync(ctx);
            if (changes > 0)
            {
                var freePlan = await db.SubscriptionMaster.FirstOrDefaultAsync(x => x.IsDefault, ctx);

                if (freePlan == null)
                {
                    trans.Rollback();
                    return null;
                }

                AccountDetails accountDetails = new()
                {
                    UserId = user.UserId,
                    SubscriptionId = freePlan.Id,
                };
                db.AccountDetails.Add(accountDetails);
                changes += await db.SaveChangesAsync(ctx);
            }
            if (changes == 2)
            {
                await trans.CommitAsync(ctx);
                return user;
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

    public async Task<bool> UpdatePassword(Guid userId, string password, string ipAddress, CancellationToken ctx = default)
    {
        using var trans = db.Database.BeginTransaction();
        try
        {
            await db.RefreshTokens.Where(re => re.UserId == userId)
                   .ExecuteUpdateAsync(r => r.SetProperty(x => x.IsRevoked, true)
                   .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                   .SetProperty(x => x.UpdatedBy, ipAddress), ctx);

            var changes = await db.UserMaster.Where(e => e.UserId == userId && e.IsDeleted == false && e.IsActive).ExecuteUpdateAsync(
            x => x.SetProperty(y => y.PasswordHash, password)
            .SetProperty(y => y.UpdatedAt, DateTime.UtcNow)
            .SetProperty(y => y.UpdatedBy, ipAddress), ctx
            );
            if (changes > 0)
                await trans.CommitAsync(ctx);
            else
                trans.Rollback();

            return changes > 0;
        }
        catch (Exception)
        {
            db.ChangeTracker.Clear();
            trans.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteProfileAsync(Guid userId, string ipAddress, CancellationToken ctx = default)
    {
        await using var trans = db.Database.BeginTransaction();
        try
        {
            await db.RefreshTokens.Where(re => re.UserId == userId)
                   .ExecuteUpdateAsync(r => r.SetProperty(x => x.IsRevoked, true)
                   .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                   .SetProperty(x => x.UpdatedBy, ipAddress), ctx);
            var changes = await db.UserMaster.Where(e => e.UserId == userId && e.IsDeleted == false && e.IsActive).ExecuteUpdateAsync(
            x => x.SetProperty(y => y.IsActive, false)
                  .SetProperty(y => y.IsDeleted, true)
                  .SetProperty(y => y.UpdatedAt, DateTime.UtcNow)
                  .SetProperty(y => y.UpdatedBy, ipAddress), ctx
            );
            if (changes > 0)
                await trans.CommitAsync(ctx);
            else
                trans.Rollback();

            return changes > 0;
        }
        catch (Exception)
        {
            db.ChangeTracker.Clear();
            trans.Rollback();
            throw;
        }
    }

    public Task<List<UserMaster>> GetAllUsersAsync(CancellationToken ctx = default)
        => db.UserMaster.AsNoTracking().Where(x => x.IsDeleted == false && x.IsActive).ToListAsync(ctx);

    public Task<UserMaster?> GetUserProfileByIdAsync(Guid userId, CancellationToken ctx = default)
        => db.UserMaster.AsNoTracking().Include(x => x.AccountDetails)
            .ThenInclude(a => a.SubscriptionMaster).FirstOrDefaultAsync(x => x.UserId == userId && x.IsDeleted == false && x.IsActive, ctx);

    public async Task<bool> UpdateUserDetailsAsync(string fullName, string email, string nationality, bool isEmailConfirmed, Guid userId, CancellationToken ctx = default)
    {
        try
        {
            var changes = await db.UserMaster.Where(e => e.UserId == userId && e.IsDeleted == false && e.IsActive).ExecuteUpdateAsync(
                u => u.SetProperty(x => x.FullName, fullName)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                .SetProperty(x => x.UpdatedBy, userId.ToString())
                .SetProperty(x => x.Email, email)
                .SetProperty(x => x.Nationality, nationality)
                .SetProperty(x => x.IsEmailConfirmed, isEmailConfirmed), ctx);

            return changes > 0;

        }
        catch (Exception)
        {
            db.ChangeTracker.Clear();
            throw;
        }
    }
}
