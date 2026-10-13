using System.Text.Json;
using api.Entities;

namespace api.Data.Import;

public class PokemonImporter
{
    private const string GameMasterUrl =
        "https://raw.githubusercontent.com/PokeMiners/game_masters/master/latest/latest.json";

    private const string PogoApiUrl = "https://pogoapi.net/api/v1/";

    // Web defaults = case-insensitive property names, so "templateId" in JSON
    // fills TemplateId in C#. Static: building options is not free, reuse one instance.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    // One client for all downloads (game master + 3 pogoapi files)
    private static readonly HttpClient Http = new();

    public async Task RunAsync()
    {
        // 1. FETCH
        var templates = await FetchTemplatesAsync();
        Console.WriteLine($"Templates loaded: {templates.Count}");

        var pogo = await FetchPogoApiAsync();
        Console.WriteLine($"pogoapi: {pogo.Names.Count} names, {pogo.Released.Count} released, {pogo.Shiny.Count} shiny");

        // 2. TRANSLATE
        var max = BuildMaxLookup(templates);
        var forms = ParseForms(templates, max);
        Console.WriteLine($"Forms parsed: {forms.Count}");

        // Only tradable real forms become Pokemon rows (costumes come in a later step)
        var pokemon = forms
            .Where(f => f.IsTradable && FormClassification.Classify(f) == FormBucket.RealForm)
            .Select(f => ToPokemon(f, pogo))
            .ToList();

        // Temporary check
        Console.WriteLine($"Pokemon rows: {pokemon.Count}, released: {pokemon.Count(p => p.IsReleased)}, " +
                          $"shiny: {pokemon.Count(p => p.ShinyAvailable)}");
        foreach (var p in pokemon.Where(p => p.DexNumber is 29 or 52 or 487 or 774 or 1017))
        {
            Console.WriteLine($"  {p.DexNumber,4} {p.FormCode,-10} {p.Name,-22} " +
                              $"released={p.IsReleased} shiny={p.ShinyAvailable} " +
                              $"dmax={p.DynamaxAvailable} gmax={p.GigantamaxAvailable}");
        }
    }

    private static async Task<List<GameMasterTemplate>> FetchTemplatesAsync()
    {
        // Stream the file instead of loading 19MB into one string first
        await using var stream = await Http.GetStreamAsync(GameMasterUrl);

        return await JsonSerializer.DeserializeAsync<List<GameMasterTemplate>>(stream, JsonOptions)
            ?? throw new Exception("Game master deserialized to null");
    }

    private static async Task<PogoApiData> FetchPogoApiAsync()
    {
        var names = await FetchPogoApiFileAsync("pokemon_names.json");
        var released = await FetchPogoApiFileAsync("released_pokemon.json");
        var shiny = await FetchPogoApiFileAsync("shiny_pokemon.json");

        return new PogoApiData(
            Names: names.ToDictionary(kv => kv.Key, kv => kv.Value.Name),
            Released: released.Keys.ToHashSet(),
            Shiny: shiny.Keys.ToHashSet());
    }

    // { "52": { "id": 52, "name": "Meowth" }, ... } -> Dictionary<int, PogoApiPokemon>
    // The JSON keys are strings ("52"); System.Text.Json converts them to int for us.
    private static async Task<Dictionary<int, PogoApiPokemon>> FetchPogoApiFileAsync(string fileName)
    {
        await using var stream = await Http.GetStreamAsync(PogoApiUrl + fileName);

        return await JsonSerializer.DeserializeAsync<Dictionary<int, PogoApiPokemon>>(stream, JsonOptions)
            ?? throw new Exception($"{fileName} deserialized to null");
    }

    // Last step of TRANSLATE: game master data + pogoapi data -> the entity, ready to save.
    // Id is left at 0 - the database assigns it.
    private static Pokemon ToPokemon(ParsedForm form, PogoApiData pogo)
    {
        if (!pogo.Names.TryGetValue(form.DexNumber, out var speciesName))
        {
            Console.WriteLine($"WARN: no pogoapi name for dex {form.DexNumber}, using {form.SpeciesCode}");
            speciesName = form.SpeciesCode;
        }

        return new Pokemon
        {
            DexNumber = form.DexNumber,
            FormCode = form.FormCode,
            Name = BuildName(speciesName, form.FormCode),
            IsReleased = pogo.Released.Contains(form.DexNumber)
                         || ReleaseOverrides.ReleasedSincePogoApi.Contains(form.DexNumber),
            ShinyAvailable = pogo.Shiny.Contains(form.DexNumber), // per species -> all its forms
            DynamaxAvailable = form.DynamaxAvailable,
            GigantamaxAvailable = form.GigantamaxAvailable,
        };
    }

    // ("Meowth", "NORMAL")   -> "Meowth"
    // ("Meowth", "ALOLA")    -> "Meowth (Alola)"
    // ("Shellos", "WEST_SEA") -> "Shellos (West Sea)"
    // Generated, not pretty everywhere ("Spinda (00)") - Name is display-only, safe to rename later.
    private static string BuildName(string speciesName, string formCode)
    {
        if (formCode == "NORMAL") return speciesName;

        var words = formCode.Split('_').Select(w => w[0] + w[1..].ToLowerInvariant());
        return $"{speciesName} ({string.Join(' ', words)})";
    }

    // (SPECIES, FORM) -> what Max this form can do, from the EXTENDED_* templates.
    // Keyed by the RAW form from the file ("MEOWTH_ALOLA"), the same value pokemonSettings has,
    // so ParseForms can look it up before cutting the species prefix off.
    // A lookup is needed here because this is a DIFFERENT template than the one ParseForms walks -
    // without it, every form would have to search all ~18.8k templates for its EXTENDED twin.
    private static Dictionary<(string Species, string? Form), (bool Dynamax, bool Gigantamax)> BuildMaxLookup(
        List<GameMasterTemplate> templates)
    {
        var lookup = new Dictionary<(string Species, string? Form), (bool Dynamax, bool Gigantamax)>();

        foreach (var template in templates)
        {
            var extended = template.Data.PokemonExtendedSettings;
            if (extended == null) continue; // not an EXTENDED template

            var modes = extended.BreadOverrides?.Select(b => b.BreadMode).ToList() ?? [];

            // Indexer, not Add(): GIMMIGHOUL and LILLIGANT have two species-level templates,
            // Add() would throw on the duplicate key. Neither is ever read (both have form templates).
            lookup[(extended.UniqueId, extended.Form)] = (
                Dynamax: modes.Contains("BREAD_MODE"),
                Gigantamax: modes.Any(m => m != null && m.StartsWith("BREAD_DOUGH_MODE")));
        }

        return lookup;
    }

    // Every species has a species-level template (V0052_POKEMON_MEOWTH, Form = null)
    // and usually one template per form (V0052_POKEMON_MEOWTH_ALOLA, ...).
    private static List<ParsedForm> ParseForms(
        List<GameMasterTemplate> templates,
        Dictionary<(string Species, string? Form), (bool Dynamax, bool Gigantamax)> maxLookup)
    {
        var result = new List<ParsedForm>();

        // Keep only pokemonSettings templates, and pull the dex out of the templateId right away
        var pokemon = templates
            .Where(t => t.Data.PokemonSettings != null)
            .Select(t => (Dex: ParseDex(t.TemplateId), Settings: t.Data.PokemonSettings!));

        // One group per species: MEOWTH -> [species template, _NORMAL, _ALOLA, _GALARIAN]
        foreach (var species in pokemon.GroupBy(p => p.Settings.PokemonId))
        {
            var formTemplates = species.Where(p => p.Settings.Form != null).ToList();

            // No form templates (Ogerpon) -> the species template becomes one NORMAL row.
            // Everyone else: the species template duplicates their _NORMAL template - skip it.
            if (formTemplates.Count == 0)
            {
                var (dex, settings) = species.First();
                // No EXTENDED template -> (false, false), the default of the tuple
                var max = maxLookup.GetValueOrDefault((species.Key, null));
                result.Add(new ParsedForm(
                    dex, species.Key, "NORMAL", settings.IsTradable, max.Dynamax, max.Gigantamax));
                continue;
            }

            foreach (var (dex, settings) in formTemplates)
            {
                var formCode = StripSpeciesPrefix(settings.PokemonId, settings.Form!);
                var max = maxLookup.GetValueOrDefault((settings.PokemonId, settings.Form));
                result.Add(new ParsedForm(
                    dex, settings.PokemonId, formCode, settings.IsTradable, max.Dynamax, max.Gigantamax));
            }
        }

        return result;
    }

    // "V0052_POKEMON_MEOWTH_ALOLA" -> the 4 digits after "V" -> "0052" -> 52
    private static int ParseDex(string templateId) => int.Parse(templateId.Substring(1, 4));

    // "MEOWTH_ALOLA" -> "ALOLA".
    // Can't just cut at the first "_" - species codes have underscores too:
    // MR_MIME_GALARIAN must give GALARIAN, not MIME_GALARIAN.
    private static string StripSpeciesPrefix(string species, string form)
    {
        var prefix = species + "_";
        if (form.StartsWith(prefix)) return form[prefix.Length..];

        // Only known case: NIDORAN_FEMALE / NIDORAN_MALE list their form as "NIDORAN_NORMAL"
        Console.WriteLine($"WARN: form {form} doesn't start with {prefix}, cutting at first '_'");
        return form[(form.IndexOf('_') + 1)..];
    }
}
