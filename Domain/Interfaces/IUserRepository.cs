using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<UserMaster?> GetByEmailAsync(string email);
    Task AddAsync(UserMaster user);
}
