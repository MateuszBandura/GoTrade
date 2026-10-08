using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Entities;

public class Costume
{
    public int Id { get; set; }

    // Full code from the game data, e.g. ANNIVERSARY_2022_NOEVOLVE
    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public string DisplayName { get; set; } = null!;

    public bool NoEvolve { get; set; }

    [JsonIgnore]
    public List<PokemonCostume> PokemonCostumes { get; set; } = [];
}
