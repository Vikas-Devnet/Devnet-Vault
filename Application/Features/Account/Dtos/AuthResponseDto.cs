namespace Application.Features.Account.Dtos;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiryDate { get; set; } = default;
    public string AccessRefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryDate { get; set; } = default;
    public DateTime CreatedDate { get; set; } = default;
}
