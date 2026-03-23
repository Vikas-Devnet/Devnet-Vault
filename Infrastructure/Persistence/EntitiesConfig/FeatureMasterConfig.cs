using Domain.Entities.Subsciption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntitiesConfig;

public class FeatureMasterConfig : IEntityTypeConfiguration<FeatureMaster>
{
    public void Configure(EntityTypeBuilder<FeatureMaster> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FeatureName)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("nvarchar(150)");

        builder.Property(f => f.Description)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.HasIndex(f => f.FeatureName)
            .IsUnique()
            .HasDatabaseName("IDX_FeatureMaster_Name");
    }
}
