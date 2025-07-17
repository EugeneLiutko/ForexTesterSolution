using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email_Unique");
            entity.HasIndex(u => u.SubscriptionId)
                .HasDatabaseName("IX_Users_SubscriptionId");
            entity.HasOne(u => u.Subscription)
                .WithOne(s => s.User)
                .HasForeignKey<User>(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_Subscriptions_SubscriptionId");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedOnAdd();
            entity.Property(s => s.Type).HasConversion<string>().IsRequired().HasMaxLength(50);
            entity.Property(s => s.StartDate).IsRequired();
            entity.Property(s => s.EndDate).IsRequired();
            entity.HasIndex(s => s.Type)
                .HasDatabaseName("IX_Subscriptions_Type");
            entity.HasIndex(s => new { s.StartDate, s.EndDate })
                .HasDatabaseName("IX_Subscriptions_DateRange");
        });
    }
    
}