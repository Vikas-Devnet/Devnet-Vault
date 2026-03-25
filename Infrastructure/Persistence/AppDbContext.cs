using Domain.Common.Models;
using Domain.Entities.Account;
using Domain.Entities.Subsciption;
using Infrastructure.Persistence.EntitiesConfig;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserMaster> UserMaster => Set<UserMaster>();
    public DbSet<RefreshTokens> RefreshTokens => Set<RefreshTokens>();
    public DbSet<AccountDetails> AccountDetails => Set<AccountDetails>();
    public DbSet<SubscriptionMaster> SubscriptionMaster => Set<SubscriptionMaster>();
    public DbSet<FeatureMaster> FeatureMaster => Set<FeatureMaster>();
    public DbSet<SubscriptionFeature> SubscriptionFeature => Set<SubscriptionFeature>();
    public DbSet<UserKey> UserKeys => Set<UserKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserMasterConfig());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfig());
        modelBuilder.ApplyConfiguration(new AccountDetailsConfig());
        modelBuilder.ApplyConfiguration(new SubsciptionMasterConfig());
        modelBuilder.ApplyConfiguration(new FeatureMasterConfig());
        modelBuilder.ApplyConfiguration(new SubscriptionFeaturesConfig());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditProperty)
                .IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(AuditProperty.CreatedAt))
                    .HasColumnType("DateTime2(3)")
                    .HasDefaultValueSql("GETUTCDATE()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(AuditProperty.UpdatedAt))
                    .HasColumnType("DateTime2(3)");

                modelBuilder.Entity(entityType.ClrType)
                   .Property(nameof(AuditProperty.CreatedBy))
                   .HasColumnType("varchar(50)")
                   .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                  .Property(nameof(AuditProperty.UpdatedBy))
                  .HasColumnType("varchar(50)");
            }
        }
    }
}
