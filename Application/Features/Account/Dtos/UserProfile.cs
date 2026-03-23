namespace Application.Features.Account.Dtos;

public class UserProfile
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
}
