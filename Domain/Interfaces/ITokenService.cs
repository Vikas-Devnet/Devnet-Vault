using Domain.Entities;

namespace Domain.Interfaces;

public interface ITokenService
{
    string Generate(UserMaster user);
}
