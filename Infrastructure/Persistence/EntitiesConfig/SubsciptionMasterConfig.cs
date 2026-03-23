using Domain.Entities.Subsciption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntitiesConfig
{
    public class SubsciptionMasterConfig : IEntityTypeConfiguration<SubscriptionMaster>
    {
        public void Configure(EntityTypeBuilder<SubscriptionMaster> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.PlanName)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnType("nvarchar(150)");

            builder.Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.Currency)
                .HasMaxLength(10)
                .HasColumnType("varchar(10)");

            builder.Property(s => s.Description)
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            builder.Property(s => s.DurationInMonths)
                .IsRequired();

            builder.Property(s => s.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(s => s.PlanName)
                .IsUnique()
                .HasDatabaseName("IDX_Subscription_PlanName");
        }
    }
}
