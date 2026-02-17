using Domain.Common.Models;
using Domain.Entities;
using Infrastructure.Persistence.EntitiesConfig;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserMaster> UserMaster => Set<UserMaster>();
    public DbSet<RefreshTokens> RefreshTokens => Set<RefreshTokens>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserMasterConfig());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfig());

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
