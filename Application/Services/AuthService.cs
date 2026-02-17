using Application.DTOs;
using Application.Interfaces;
using Application.Models.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService(IUserRepository users, IPasswordHasher hasher, IJwtTokenGenerator jwtTokenGenerator)
{
    public async Task<ServiceResponseGenerator<UserMaster>> Signup(SignupDto dto)
    {
        var existing = await users.GetByEmailAsync(dto.Email);
        if (existing != null) return ServiceResponseGenerator<UserMaster>.Failure("Email already Exists");

        var user = new UserMaster
        {
            Email = dto.Email.Trim(),
            PasswordHash = hasher.Hash(dto.Password.Trim()),
            FullName = dto.FullName.Trim(),
            Nationality = dto.Nationlity.Trim(),
            CreatedBy = "SignedUp"
        };

        await users.AddAsync(user);
        return ServiceResponseGenerator<UserMaster>.Success("User Registered Successfully", user);
    }

    public async Task<ServiceResponseGenerator<object>> Login(LoginUserDto dto)
    {
        var user = await users.GetByEmailAsync(dto.Email);
        if (user == null) return ServiceResponseGenerator<object>.Failure("Email not valid");

        if (!hasher.Verify(dto.Password, user.PasswordHash))
            return ServiceResponseGenerator<object>.Failure("Invalid password.");

        var token = jwtTokenGenerator.GenerateToken(user.UserId, user.Email);
        if (string.IsNullOrEmpty(token))
            return ServiceResponseGenerator<object>.Failure("Failed to generate security token");

        return ServiceResponseGenerator<object>.Success("Credentials Verified Successfully", new
        {
            Token = token
        });
    }
}
