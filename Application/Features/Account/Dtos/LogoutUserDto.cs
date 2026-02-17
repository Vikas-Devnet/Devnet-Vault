using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Dtos;

public class LogoutUserDto
{
    public bool LogOutAllDevices { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 6)]
    public required string IpAddress { get; set; }
}
