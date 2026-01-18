using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<UserMaster?> GetByEmailAsync(string email)
        => _db.UserMaster.FirstOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(UserMaster user)
    {
        _db.UserMaster.Add(user);
        await _db.SaveChangesAsync();
    }
}
