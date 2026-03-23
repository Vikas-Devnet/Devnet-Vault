using Application.Features.Account.Dtos;
using Application.Features.Common.Models;

namespace Application.Features.Account.Interfaces;

public interface IProfileService
{
    Task<ServiceResponseGenerator<bool>> DeleteProfileAsync(Guid id, string ipAddress, CancellationToken ctx = default);
    Task<ServiceResponseGenerator<List<UserProfile>>> GetAllProfilesAsync(CancellationToken ctx = default);
    Task<ServiceResponseGenerator<UserProfile>> GetProfileAsync(Guid id, CancellationToken ctx = default);
    Task<ServiceResponseGenerator<UserProfile>> UpdateProfileAsync(UserProfile profile, Guid id, CancellationToken ctx = default);
}
