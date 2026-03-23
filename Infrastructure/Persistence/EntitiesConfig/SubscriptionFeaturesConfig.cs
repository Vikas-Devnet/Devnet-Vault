using Domain.Entities.Subsciption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntitiesConfig;

public class SubscriptionFeaturesConfig : IEntityTypeConfiguration<SubscriptionFeature>
{
    public void Configure(EntityTypeBuilder<SubscriptionFeature> builder)
    {
        builder.HasKey(sf => sf.Id);

        builder.HasIndex(sf => new { sf.SubscriptionId, sf.FeatureId })
            .IsUnique()
            .HasDatabaseName("IDX_Subscription_Feature");

        builder.HasOne(sf => sf.SubscriptionMaster)
            .WithMany(s => s.SubscriptionFeatures)
            .HasForeignKey(sf => sf.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_SubscriptionFeatures_Subscription");

        builder.HasOne(sf => sf.FeatureMaster)
            .WithMany(f => f.SubscriptionFeatures)
            .HasForeignKey(sf => sf.FeatureId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_SubscriptionFeatures_Feature");
    }
}
