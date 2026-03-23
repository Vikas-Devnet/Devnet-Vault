using Application.Features.Account.Dtos;
using Application.Features.Common.Models;

namespace Application.Features.Account.Interfaces;

public interface IProfileService
{
    Task<ServiceResponseGenerator<bool>> DeleteProfileAsync(Guid id, string ipAddress, CancellationToken ctx = default);
    Task<ServiceResponseGenerator<List<UserProfileResponseDto>>> GetAllProfilesAsync(CancellationToken ctx = default);
    Task<ServiceResponseGenerator<UserProfileResponseDto>> GetProfileAsync(Guid id, CancellationToken ctx = default);
    Task<ServiceResponseGenerator<bool>> UpdateProfileAsync(UserProfileUpdateDto profile, Guid id, CancellationToken ctx = default);
}
