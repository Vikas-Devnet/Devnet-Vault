using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService(IUserRepository users, IPasswordHasher hasher, ITokenService jwt)
{
    public async Task<AuthResultDto> Signup(SignupDto dto)
    {
        var existing = await users.GetByEmailAsync(dto.Email);
        if (existing != null) return AuthResultDto.Fail("Email already registered.");

        var user = new UserMaster
        {
            Email = dto.Email,
            PasswordHash = hasher.Hash(dto.Password)
        };

        await users.AddAsync(user);
        var token = jwt.Generate(user);
        return AuthResultDto.Success(token);
    }

    public async Task<AuthResultDto> Login(LoginUserDto dto)
    {
        var user = await users.GetByEmailAsync(dto.Email);
        if (user == null) return AuthResultDto.Fail("Invalid email.");

        if (!hasher.Verify(dto.Password, user.PasswordHash))
            return AuthResultDto.Fail("Invalid password.");

        var token = jwt.Generate(user);
        return AuthResultDto.Success(token);
    }
}
