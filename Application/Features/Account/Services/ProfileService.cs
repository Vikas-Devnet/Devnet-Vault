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

    public async Task<ServiceResponseGenerator<List<UserProfileResponseDto>>> GetAllProfilesAsync(CancellationToken ctx = default)
    {
        var usersProfile = await users.GetAllUsersAsync(ctx);
        var profiles = usersProfile.Select(u => new UserProfileResponseDto
        {
            FullName = u.FullName,
            Email = u.Email,
            Nationality = u.Nationality,
            IsEmailConfirmed = u.IsEmailConfirmed
        }).ToList();
        return ServiceResponseGenerator<List<UserProfileResponseDto>>.Success("Profiles retrieved successfully", profiles);
    }

    public async Task<ServiceResponseGenerator<UserProfileResponseDto>> GetProfileAsync(Guid id, CancellationToken ctx = default)
    {
        var userProfile = await users.GetUserProfileByIdAsync(id, ctx);
        if (userProfile is null)
            return ServiceResponseGenerator<UserProfileResponseDto>.Failure("Profile not found");
        var profile = new UserProfileResponseDto()
        {
            FullName = userProfile.FullName,
            Email = userProfile.Email,
            Nationality = userProfile.Nationality,
            IsEmailConfirmed = userProfile.IsEmailConfirmed,
            SubscriptionId = userProfile.AccountDetails.SubscriptionId,
            SubscriptionName = userProfile.AccountDetails.SubscriptionMaster.PlanName,
            SubscriptionExpiry = userProfile.AccountDetails.ExpiryDate

        };
        return ServiceResponseGenerator<UserProfileResponseDto>.Success("Profile retrieved successfully", profile);
    }

    public async Task<ServiceResponseGenerator<bool>> UpdateProfileAsync(UserProfileUpdateDto profile, Guid id, CancellationToken ctx = default)
    {
        if (profile.FullName is null || profile.Email is null || profile.Nationality is null)
            return ServiceResponseGenerator<bool>.Failure("Invalid profile data");

        var userDetails = await users.GetByEmailAsync(profile.Email, ctx);

        if (userDetails is not null && userDetails.UserId != id)
            return ServiceResponseGenerator<bool>.Failure("Email already in use");

        var isUpdated = await users.UpdateUserDetailsAsync(profile.FullName, profile.Email, profile.Nationality, profile.IsEmailConfirmed, id, ctx);
        if (isUpdated)
            return ServiceResponseGenerator<bool>.Success("Profile updated successfully", true);
        return ServiceResponseGenerator<bool>.Failure("Failed to update profile");
    }
}
