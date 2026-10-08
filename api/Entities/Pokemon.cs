using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Entities;

// One row = one FORM (Meowth NORMAL, Meowth ALOLA, ...).
// Natural key is (DexNumber, FormCode) - the importer upserts by that, never by Id.
public class Pokemon
{
    public int Id { get; set; }

    public int DexNumber { get; set; }

    [Required]
    public string FormCode { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    public bool IsReleased { get; set; }

    public bool ShinyAvailable { get; set; }

    public bool DynamaxAvailable { get; set; }

    public bool GigantamaxAvailable { get; set; }

    [JsonIgnore]
    public List<PokemonCostume> PokemonCostumes { get; set; } = [];
}
