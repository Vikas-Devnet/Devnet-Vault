using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<UserMaster?> GetByEmailAsync(string email)
        => db.UserMaster.FirstOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(UserMaster user)
    {
        db.UserMaster.Add(user);
        await db.SaveChangesAsync();
    }
}
