using System.Text.Json.Serialization;
using api.Enums;

namespace api.Entities;

// One Pokemon a trainer offers (Have) or looks for (Want).
// Nullable attributes: on a Have always set (CHECK in AppDbContext), on a Want null = don't care.
// Duplicates are allowed - a trainer can offer or want several copies of the same Pokemon.
public class TradeListing
{
    public int Id { get; set; }

    public string TrainerId { get; set; } = null!;

    public int PokemonId { get; set; }

    public ListingKind Kind { get; set; }

    public bool? IsShiny { get; set; }

    public bool? HasCostume { get; set; }

    // Optional even when HasCostume = true: a brand new costume may not be in our list yet,
    // the trainer then describes it in Notes
    public int? CostumeId { get; set; }

    public MaxType? MaxType { get; set; }

    public bool? HasBackground { get; set; }

    public string? Notes { get; set; }

    public ListingStatus Status { get; set; } = ListingStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Trainer Trainer { get; set; } = null!;

    [JsonIgnore]
    public Pokemon Pokemon { get; set; } = null!;

    // Reached through (PokemonId, CostumeId). The costume itself: PokemonCostume.Costume
    [JsonIgnore]
    public PokemonCostume? PokemonCostume { get; set; }
}
