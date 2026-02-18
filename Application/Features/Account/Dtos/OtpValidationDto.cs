using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Dtos;

public class OtpValidationDto
{
    [Required(ErrorMessage = "Otp is Required")]
    [StringLength(10, MinimumLength = 3)]
    public string Otp { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 3)]
    public string OtpKey { get; set; } = string.Empty;
}
