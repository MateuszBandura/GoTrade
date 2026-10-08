using System.Text.Json.Serialization;
using api.Enums;

namespace api.Entities;

// Junction: which Pokemon (form) exists in which costume.
// PK = (PokemonId, CostumeId), configured in AppDbContext.
public class PokemonCostume
{
    public int PokemonId { get; set; }

    public int CostumeId { get; set; }

    public SpriteCode SpriteCode { get; set; }

    [JsonIgnore]
    public Pokemon Pokemon { get; set; } = null!;

    [JsonIgnore]
    public Costume Costume { get; set; } = null!;
}
