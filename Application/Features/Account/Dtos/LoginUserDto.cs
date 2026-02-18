using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Dtos;

public class LoginUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(200, MinimumLength = 5)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{6,}$")]
    public required string Password { get; set; }
}
