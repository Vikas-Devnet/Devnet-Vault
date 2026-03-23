using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntitiesConfig;

public class AccountDetailsConfig : IEntityTypeConfiguration<AccountDetails>
{
    public void Configure(EntityTypeBuilder<AccountDetails> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.IsAdmin)
            .IsRequired();

        builder.Property(a => a.ExpiryDate)
            .HasColumnType("datetime");

        builder.HasIndex(a => a.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_AccountDetails_UserId");

        builder.HasOne(a => a.UserMaster)
            .WithOne(u => u.AccountDetails)
            .HasForeignKey<AccountDetails>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_AccountDetails_UserMaster");

        builder.HasOne(a => a.SubscriptionMaster)
            .WithMany(s => s.AccountDetails)
            .HasForeignKey(a => a.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_AccountDetails_Subscription");
    }
}
