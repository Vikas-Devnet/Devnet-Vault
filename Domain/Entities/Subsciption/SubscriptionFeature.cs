using Domain.Common.Models;

namespace Domain.Entities.Subsciption;

public class SubscriptionFeature : AuditProperty
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public int FeatureId { get; set; }
    public SubscriptionMaster SubscriptionMaster { get; set; } = null!;
    public FeatureMaster FeatureMaster { get; set; } = null!;
}
