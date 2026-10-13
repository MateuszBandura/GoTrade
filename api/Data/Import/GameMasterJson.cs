namespace api.Data.Import;

// Classes that mirror the SHAPE of latest.json (PokeMiners game master).
// They are not entities - nothing here goes to the database directly.
// Only the fields we actually read are declared; the JSON has far more,
// and System.Text.Json silently skips anything without a matching property.

// One element of the top-level array:
// { "templateId": "V0052_POKEMON_MEOWTH_ALOLA", "data": { ... } }
public class GameMasterTemplate
{
    public string TemplateId { get; set; } = null!;

    public GameMasterData Data { get; set; } = null!;
}

// "data" holds a DIFFERENT key depending on the template type
// (pokemonSettings, pokemonExtendedSettings, ...).
// Every property is nullable - for a given template only one of them is set.
public class GameMasterData
{
    public PokemonSettings? PokemonSettings { get; set; }

    public PokemonExtendedSettings? PokemonExtendedSettings { get; set; }
}

// From V0052_POKEMON_MEOWTH_ALOLA (one form) or V0052_POKEMON_MEOWTH (whole species, no "form").
// "pokemonSettings": { "pokemonId": "MEOWTH", "form": "MEOWTH_ALOLA", "isTradable": true, ... }
public class PokemonSettings
{
    public string PokemonId { get; set; } = null!;

    // null on the species-level template
    public string? Form { get; set; }

    // The file OMITS false values (Mew has no "isTradable" key at all).
    // A missing key leaves the C# default - and the default of bool is false. Exactly what we want.
    public bool IsTradable { get; set; }
}

// From EXTENDED_V0052_POKEMON_MEOWTH_ALOLA - same (species, form) split as pokemonSettings.
// "pokemonExtendedSettings": {
//     "uniqueId": "MEOWTH", "form": "MEOWTH_ALOLA",
//     "breadOverrides": [ { "breadMode": "BREAD_MODE" }, { "breadMode": "BREAD_DOUGH_MODE" } ]
// }
// "bread" is Niantic's codename for Max battles:
//   BREAD_MODE       = Dynamax
//   BREAD_DOUGH_MODE = Gigantamax (Urshifu Single Strike has BREAD_DOUGH_MODE_2)
public class PokemonExtendedSettings
{
    // The species - same value as PokemonSettings.PokemonId, just a different name in this template
    public string UniqueId { get; set; } = null!;

    // null on the species-level template
    public string? Form { get; set; }

    // Missing = this form has no Max at all
    public List<BreadOverride>? BreadOverrides { get; set; }
}

public class BreadOverride
{
    // Nullable: Cottonee has an override with no breadMode
    public string? BreadMode { get; set; }
}
