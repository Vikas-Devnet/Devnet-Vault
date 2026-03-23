using System.ComponentModel.DataAnnotations;

namespace Application.Features.Account.Dtos;

public class UserProfileUpdateDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(150, ErrorMessage = "Full name cannot exceed 150 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nationality is required")]
    [MaxLength(100, ErrorMessage = "Nationality cannot exceed 100 characters")]
    public string Nationality { get; set; } = string.Empty;

    public bool IsEmailConfirmed { get; set; }
}
