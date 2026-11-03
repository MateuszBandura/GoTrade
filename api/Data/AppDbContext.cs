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

        // TradeListing
        builder.Entity<TradeListing>()
                .Property(l => l.Kind)
                .HasConversion<string>();

        builder.Entity<TradeListing>()
                .Property(l => l.MaxType)
                .HasConversion<string>();

        builder.Entity<TradeListing>()
                .Property(l => l.Status)
                .HasConversion<string>();

        // Restrict everywhere: listings are never deleted, and nothing may delete them by cascade
        builder.Entity<TradeListing>()
                .HasOne(l => l.Trainer)
                .WithMany()
                .HasForeignKey(l => l.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TradeListing>()
                .HasOne(l => l.Pokemon)
                .WithMany()
                .HasForeignKey(l => l.PokemonId)
                .OnDelete(DeleteBehavior.Restrict);

        // Composite FK: the DB rejects a costume this Pokemon doesn't have.
        // Postgres skips the check when CostumeId is NULL
        builder.Entity<TradeListing>()
                .HasOne(l => l.PokemonCostume)
                .WithMany()
                .HasForeignKey(l => new { l.PokemonId, l.CostumeId })
                .OnDelete(DeleteBehavior.Restrict);

        // For matching: "active Haves / Wants of this Pokemon"
        builder.Entity<TradeListing>()
                .HasIndex(l => new { l.PokemonId, l.Kind, l.Status });

        builder.Entity<TradeListing>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_TradeListing_CostumeRequiresHasCostume",
                        "\"CostumeId\" IS NULL OR \"HasCostume\" = true");

                    t.HasCheckConstraint(
                        "CK_TradeListing_HaveAttributesSet",
                        "\"Kind\" <> 'Have' OR (\"IsShiny\" IS NOT NULL AND \"HasCostume\" IS NOT NULL " +
                        "AND \"MaxType\" IS NOT NULL AND \"HasBackground\" IS NOT NULL)");
                });

        // Trade
        builder.Entity<Trade>()
                .Property(t => t.Status)
                .HasConversion<string>();

        // Two FKs to the same table: EF can't guess which navigation goes with which FK,
        // so each pair is spelled out. Restrict: trade history must never disappear by cascade
        builder.Entity<Trade>()
                .HasOne(t => t.Initiator)
                .WithMany()
                .HasForeignKey(t => t.InitiatorId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trade>()
                .HasOne(t => t.Responder)
                .WithMany()
                .HasForeignKey(t => t.ResponderId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trade>()
                .HasOne(t => t.InitiatorListing)
                .WithMany()
                .HasForeignKey(t => t.InitiatorListingId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trade>()
                .HasOne(t => t.ResponderListing)
                .WithMany()
                .HasForeignKey(t => t.ResponderListingId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trade>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Trade_DifferentTrainers",
                    "\"InitiatorId\" <> \"ResponderId\""));

        // TradeMessage
        builder.Entity<TradeMessage>()
                .HasOne(m => m.Trade)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TradeId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TradeMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

        // For loading a conversation in order
        builder.Entity<TradeMessage>()
                .HasIndex(m => new { m.TradeId, m.SentAt });

    }

    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Pokemon> Pokemon { get; set; }
    public DbSet<Costume> Costumes { get; set; }
    public DbSet<PokemonCostume> PokemonCostumes { get; set; }
    public DbSet<TradeListing> TradeListings { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<TradeMessage> TradeMessages { get; set; }
}
