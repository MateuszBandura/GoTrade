using api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

// IdentityDbContext<AppUser> brings in all the Identity tables
// (AspNetUsers, AspNetRoles, AspNetUserClaims, ...) configured for our
// AppUser. Add your own DbSet<T> properties below for trade entities later.
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // AppUser
        builder.Entity<AppUser>()
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique();

        // Trainer
        builder.Entity<Trainer>()
                .Property(t => t.Team)
                .HasConversion<string>();

        builder.Entity<Trainer>()
                .HasOne(t => t.User)
                .WithOne(u => u.Trainer)
                .HasForeignKey<Trainer>(t => t.Id);

    }
    
    public DbSet<Trainer> Trainers { get; set; }
}
