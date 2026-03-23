using Application.Features.Account.Dtos;
using Application.Features.Account.Interfaces;
using Application.Features.Common.Models;
using Domain.Interfaces;

namespace Application.Features.Account.Services;

public class ProfileService(IUserRepository users) : IProfileService
{
    public async Task<ServiceResponseGenerator<bool>> DeleteProfileAsync(Guid id, string ipAddress, CancellationToken ctx = default)
    {
        var isDeleted = await users.DeleteProfileAsync(id, ipAddress, ctx);
        if (isDeleted)
            return ServiceResponseGenerator<bool>.Success("Profile deleted successfully", true);

        return ServiceResponseGenerator<bool>.Failure("Failed to delete profile");
    }

    public async Task<ServiceResponseGenerator<List<UserProfile>>> GetAllProfilesAsync(CancellationToken ctx = default)
    {
        var usersProfile = await users.GetAllUsersAsync(ctx);
        var profiles = usersProfile.Select(u => new UserProfile
        {
            FullName = u.FullName,
            Email = u.Email,
            Nationality = u.Nationality,
            IsEmailConfirmed = u.IsEmailConfirmed
        }).ToList();
        return ServiceResponseGenerator<List<UserProfile>>.Success("Profiles retrieved successfully", profiles);
    }

    public async Task<ServiceResponseGenerator<UserProfile>> GetProfileAsync(Guid id, CancellationToken ctx = default)
    {
        var userProfile = await users.GetUserByIdAsync(id, ctx);
        if (userProfile is null)
            return ServiceResponseGenerator<UserProfile>.Failure("Profile not found");
        var profile = new UserProfile()
        {
            FullName = userProfile.FullName,
            Email = userProfile.Email,
            Nationality = userProfile.Nationality,
            IsEmailConfirmed = userProfile.IsEmailConfirmed
        };
        return ServiceResponseGenerator<UserProfile>.Success("Profile retrieved successfully", profile);
    }

    public async Task<ServiceResponseGenerator<UserProfile>> UpdateProfileAsync(UserProfile profile, Guid id, CancellationToken ctx = default)
    {
        if (profile.FullName is null || profile.Email is null || profile.Nationality is null)
            return ServiceResponseGenerator<UserProfile>.Failure("Invalid profile data");
        var isUpdated = await users.UpdateUserDetailsAsync(profile.FullName, profile.Email, profile.Nationality, profile.IsEmailConfirmed, id, ctx);
        if (isUpdated)
            return ServiceResponseGenerator<UserProfile>.Success("Profile updated successfully", profile);
        return ServiceResponseGenerator<UserProfile>.Failure("Failed to update profile");
    }
}
