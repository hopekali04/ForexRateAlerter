using Microsoft.EntityFrameworkCore;
using ForexRateAlerter.Core.Models;

namespace ForexRateAlerter.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<ExchangeRateHistory> ExchangeRateHistory { get; set; }
        public DbSet<AlertLog> AlertLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            // Alert configuration
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BaseCurrency).HasMaxLength(3).IsFixedLength();
                entity.Property(e => e.TargetCurrency).HasMaxLength(3).IsFixedLength();
                entity.Property(e => e.TargetRate).HasPrecision(18, 6);
                
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Alerts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.BaseCurrency, e.TargetCurrency });
            });

            // ExchangeRate configuration
            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BaseCurrency).HasMaxLength(3).IsFixedLength();
                entity.Property(e => e.TargetCurrency).HasMaxLength(3).IsFixedLength();
                entity.Property(e => e.Rate).HasPrecision(18, 6);
                entity.Property(e => e.Source).HasMaxLength(50);
                
                entity.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency, e.Timestamp });
            });

            // ExchangeRateHistory configuration
            modelBuilder.Entity<ExchangeRateHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BaseCurrency).HasMaxLength(3).IsFixedLength();
                entity.Property(e => e.TargetCurrency).HasMaxLength(3).IsFixedLength();
                entity.Property(e => e.Rate).HasPrecision(18, 6);
                entity.Property(e => e.Source).HasMaxLength(50);
                
                // Index for fast time-based queries
                entity.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency, e.CreatedAt });
            });

            // AlertLog configuration
            modelBuilder.Entity<AlertLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TriggeredRate).HasPrecision(18, 6);
                entity.Property(e => e.TargetRate).HasPrecision(18, 6);
                
                entity.HasOne(e => e.Alert)
                    .WithMany(e => e.AlertLogs)
                    .HasForeignKey(e => e.AlertId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
