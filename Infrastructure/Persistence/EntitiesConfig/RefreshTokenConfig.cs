using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntitiesConfig;

public class RefreshTokenConfig:IEntityTypeConfiguration<RefreshTokens>
{
    public void Configure(EntityTypeBuilder<RefreshTokens> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.RefreshToken)
            .IsRequired()
            .HasColumnType("nvarchar(200)");

        builder.Property(t => t.RefreshToken)
           .IsRequired()
           .HasColumnType("nvarchar(200)");

        builder.Property(t => t.IPAddress)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.HasIndex(t => t.RefreshToken)
            .IsUnique()
            .HasDatabaseName("IDX_RefreshToken");

        builder.HasOne(t => t.UserMaster)
           .WithMany(u => u.RefreshTokens)
           .HasForeignKey(t => t.UserId)
           .OnDelete(DeleteBehavior.Cascade)
           .HasConstraintName("FK_RefreshTokens_UserMaster");
    }
}
