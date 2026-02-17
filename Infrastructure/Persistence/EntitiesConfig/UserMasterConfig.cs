using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntitiesConfig;

public class UserMasterConfig : IEntityTypeConfiguration<UserMaster>
{
    public void Configure(EntityTypeBuilder<UserMaster> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.UserId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(x => x.Email)
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.Nationality)
            .HasColumnType("varchar(60)")
            .IsRequired();

        builder.HasIndex(x => x.Email)
           .IsUnique()
           .HasDatabaseName("IX_UserMaster_Email");
    }
}
