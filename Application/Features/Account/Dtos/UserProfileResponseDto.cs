namespace Application.Features.Account.Dtos;

public class UserProfileResponseDto : UserProfileUpdateDto
{
    public int SubscriptionId { get; set; }
    public string SubscriptionName { get; set; } = string.Empty;
    public DateTime? SubscriptionExpiry { get; set; }
}
