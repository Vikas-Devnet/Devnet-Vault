using Application.Features.Account.Dtos;
using Application.Features.Account.Interfaces;
using Application.Features.Common.Models;

namespace Application.Features.Account.Services;

public class ProfileService : IProfileService
{
    public Task<ServiceResponseGenerator<bool>> DeleteProfileAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponseGenerator<IEnumerable<UserProfile>>> GetAllProfilesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponseGenerator<UserProfile>> GetProfileAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponseGenerator<UserProfile>> UpdateProfileAsync(UserProfile profile)
    {
        throw new NotImplementedException();
    }
}
