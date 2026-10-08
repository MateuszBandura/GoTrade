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

        // Pokemon
        builder.Entity<Pokemon>()
                .HasIndex(p => new { p.DexNumber, p.FormCode })
                .IsUnique();

        // Costume
        builder.Entity<Costume>()
                .HasIndex(c => c.Code)
                .IsUnique();

        // PokemonCostume (M:N junction with a payload column)
        builder.Entity<PokemonCostume>()
                .HasKey(pc => new { pc.PokemonId, pc.CostumeId });

        builder.Entity<PokemonCostume>()
                .HasOne(pc => pc.Pokemon)
                .WithMany(p => p.PokemonCostumes)
                .HasForeignKey(pc => pc.PokemonId);

        builder.Entity<PokemonCostume>()
                .HasOne(pc => pc.Costume)
                .WithMany(c => c.PokemonCostumes)
                .HasForeignKey(pc => pc.CostumeId);

        builder.Entity<PokemonCostume>()
                .Property(pc => pc.SpriteCode)
                .HasConversion<string>();

    }

    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Pokemon> Pokemon { get; set; }
    public DbSet<Costume> Costumes { get; set; }
    public DbSet<PokemonCostume> PokemonCostumes { get; set; }
}
