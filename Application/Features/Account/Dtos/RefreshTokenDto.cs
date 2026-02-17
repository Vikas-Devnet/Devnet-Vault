using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Dtos;

public class RefreshTokenDto
{
    [Required]
    [StringLength(200, MinimumLength = 6)]
    public required string RefreshToken { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 6)]
    public required string IPAddress { get; set; }
}
