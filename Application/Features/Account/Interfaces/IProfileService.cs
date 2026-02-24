using Application.Features.Account.Dtos;
using Application.Features.Common.Models;

namespace Application.Features.Account.Interfaces;

public interface IProfileService
{
    Task<ServiceResponseGenerator<UserProfile>> GetProfileAsync(Guid id);
    Task<ServiceResponseGenerator<IEnumerable<UserProfile>>> GetAllProfilesAsync();
    Task<ServiceResponseGenerator<UserProfile>> UpdateProfileAsync(UserProfile profile);
    Task<ServiceResponseGenerator<bool>> DeleteProfileAsync(Guid id);
}
