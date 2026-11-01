using System.Text.Json;
using api.Data.Import.Json;
using api.Data.Import.Parsed;
using api.Data.Import.Rules;
using api.Entities;
using api.Enums;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Import;

// AppDbContext comes from DI - that's why Program.cs creates a scope before resolving the importer
public class PokemonImporter(AppDbContext db)
{
    private const string GameMasterUrl =
        "https://raw.githubusercontent.com/PokeMiners/game_masters/master/latest/latest.json";

    private const string PogoApiUrl = "https://pogoapi.net/api/v1/";

    // "branch:folder" = the file list of one folder in a single request (no need to walk the repo)
    private const string SpriteTreeUrl =
        "https://api.github.com/repos/PokeMiners/pogo_assets/git/trees/master:Images/Pokemon/Addressable%20Assets";

    // Web defaults = case-insensitive property names, so "templateId" in JSON
    // fills TemplateId in C#. Static: building options is not free, reuse one instance.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    // One client for all downloads (game master, 3 pogoapi files, sprite list)
    private static readonly HttpClient Http = CreateHttpClient();

    // GitHub's API rejects requests without a User-Agent (403). raw.githubusercontent.com
    // and pogoapi don't care, so one header for everything is fine.
    private static HttpClient CreateHttpClient()
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("GoTrade-importer");
        return http;
    }

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

        // Only tradable real forms become Pokemon rows (costume forms go to Costume, below)
        var pokemon = forms
            .Where(f => f.IsTradable && FormClassification.Classify(f) == FormBucket.RealForm)
            .Select(f => ToPokemon(f, pogo))
            .ToList();

        Console.WriteLine($"Pokemon rows: {pokemon.Count}, released: {pokemon.Count(p => p.IsReleased)}, " +
                          $"shiny: {pokemon.Count(p => p.ShinyAvailable)}");

        // 3. SAVE
        await SavePokemonAsync(pokemon);

        // COSTUMES - after the Pokemon, because the links point at Pokemon rows
        // 1. FETCH
        var spriteCostumes = ParseSpriteCostumes(await FetchSpriteFileNamesAsync());

        // 2. TRANSLATE
        var links = BuildCostumeLinks(forms, spriteCostumes);
        var costumes = links.Select(l => l.CostumeCode).Distinct().Select(ToCostume).ToList();
        Console.WriteLine($"Costumes: {costumes.Count}, links: {links.Count} " +
                          $"(F: {links.Count(l => l.SpriteCode == SpriteCode.F)}, " +
                          $"C: {links.Count(l => l.SpriteCode == SpriteCode.C)})");

        // 3. SAVE
        await SaveCostumesAsync(costumes, links);
    }

    // Same upsert idea as SavePokemonAsync, for two tables in one go:
    //   Costume         by Code
    //   PokemonCostume  by (PokemonId, CostumeId)
    // Never deletes. One SaveChanges at the end = costumes and links in ONE transaction.
    private async Task SaveCostumesAsync(List<Costume> imported, List<CostumeLink> links)
    {
        // --- Costume ---
        var costumeByCode = await db.Costumes.ToDictionaryAsync(c => c.Code);
        var addedCostumes = 0;

        foreach (var c in imported)
        {
            if (costumeByCode.TryGetValue(c.Code, out var row))
            {
                row.DisplayName = c.DisplayName;
                row.NoEvolve = c.NoEvolve;
            }
            else
            {
                db.Costumes.Add(c);
                costumeByCode[c.Code] = c; // still Id = 0 - the links below use the object, not the Id
                addedCostumes++;
            }
        }

        // --- PokemonCostume ---
        // Only the key -> Id mapping is needed, not whole Pokemon.
        // Select BEFORE ToDictionary = the SQL asks for these 3 columns only
        var pokemonIds = await db.Pokemon
            .Select(p => new { p.DexNumber, p.FormCode, p.Id })
            .ToDictionaryAsync(p => (p.DexNumber, p.FormCode), p => p.Id);
        var existingLinks = await db.PokemonCostumes.ToDictionaryAsync(pc => (pc.PokemonId, pc.CostumeId));
        var addedLinks = 0;

        foreach (var link in links)
        {
            if (!pokemonIds.TryGetValue((link.DexNumber, link.FormCode), out var pokemonId))
            {
                Console.WriteLine($"WARN: no Pokemon row ({link.DexNumber}, {link.FormCode}) " +
                                  $"for costume {link.CostumeCode}, skipped");
                continue;
            }

            var costume = costumeByCode[link.CostumeCode];

            // Id = 0 -> the costume is new in this run, so it can't have any links in the DB yet
            if (costume.Id != 0 && existingLinks.TryGetValue((pokemonId, costume.Id), out var row))
            {
                row.SpriteCode = link.SpriteCode;
            }
            else
            {
                // Costume = the object, not CostumeId = number: a new costume has no Id yet.
                // EF sees the navigation, inserts the costume first and fills CostumeId in itself.
                db.PokemonCostumes.Add(new PokemonCostume
                {
                    PokemonId = pokemonId,
                    Costume = costume,
                    SpriteCode = link.SpriteCode,
                });
                addedLinks++;
            }
        }

        var written = await db.SaveChangesAsync();
        Console.WriteLine($"Saved: {addedCostumes} costumes added, {addedLinks} links added, " +
                          $"{written - addedCostumes - addedLinks} rows updated");
    }

    // Both costume sources -> one list of links, each pointing at a Pokemon row by (DexNumber, FormCode)
    private static List<CostumeLink> BuildCostumeLinks(List<GameMasterForm> forms, List<SpriteCostume> spriteCostumes)
    {
        // F: the game master lists them as forms (PIKACHU_WCS_2022). The form code IS the costume code,
        // and the costume sits on the species' NORMAL row
        var fromForms = forms
            .Where(f => f.IsTradable && FormClassification.Classify(f) == FormBucket.Costume)
            .Select(f => new CostumeLink(f.DexNumber, "NORMAL", f.FormCode, SpriteCode.F));

        // C: from the sprite names. No form in the name = the NORMAL row
        var fromSprites = spriteCostumes
            .Select(c => new CostumeLink(c.DexNumber, CostumeRowForm(c.FormCode), c.CostumeCode, SpriteCode.C));

        // Distinct: the 4 Pumpkaboo sizes of one costume are now the same link (see CostumeRowForm)
        return fromForms.Concat(fromSprites).Distinct().ToList();
    }

    // Which Pokemon row a sprite costume belongs to.
    // Sizes are skipped as forms (FormClassification), AVERAGE is the one row left for them.
    private static string CostumeRowForm(string? spriteForm) => spriteForm switch
    {
        null => "NORMAL",
        "SMALL" or "LARGE" or "SUPER" => "AVERAGE",
        _ => spriteForm,
    };

    // "JAN_2020_NOEVOLVE" -> Code = JAN_2020_NOEVOLVE, DisplayName = "Jan 2020", NoEvolve = true
    // NoEvolve is part of the code, not of the name: the costume looks the same, it just can't evolve.
    private static Costume ToCostume(string code)
    {
        const string noEvolveSuffix = "_NOEVOLVE";
        var noEvolve = code.EndsWith(noEvolveSuffix);
        var nameCode = noEvolve ? code[..^noEvolveSuffix.Length] : code;

        return new Costume
        {
            Code = code,
            DisplayName = TitleCase(nameCode),
            NoEvolve = noEvolve,
        };
    }

    private static async Task<List<string>> FetchSpriteFileNamesAsync()
    {
        await using var stream = await Http.GetStreamAsync(SpriteTreeUrl);

        var tree = await JsonSerializer.DeserializeAsync<GitHubTree>(stream, JsonOptions)
            ?? throw new Exception("Sprite tree deserialized to null");

        // Fail loudly - a cut-off list would silently lose costumes
        if (tree.Truncated) throw new Exception("GitHub truncated the sprite list");

        return tree.Tree.Select(e => e.Path).ToList();
    }

    // File name = parts separated by '.', the order of the middle ones varies:
    //   pm77 . fGALARIAN . cGOFEST_2021_NOEVOLVE . s . icon . png
    //   dex    form        costume                 shiny  (ignored)
    // Other parts we ignore: g2 (female), s (shiny), icon / portrait, png.
    private static List<SpriteCostume> ParseSpriteCostumes(List<string> fileNames)
    {
        var result = new List<SpriteCostume>();

        foreach (var fileName in fileNames)
        {
            var parts = fileName.Split('.');
            if (!parts[0].StartsWith("pm")) continue;

            // "c..." part = costume. No costume -> plain form sprite, not our business here
            var costume = parts.FirstOrDefault(p => p.StartsWith('c'));
            if (costume == null) continue;

            // "pm479.f.icon.png" exists - an empty "f" means no form, same as no "f" part
            var form = parts.FirstOrDefault(p => p.StartsWith('f') && p.Length > 1);

            result.Add(new SpriteCostume(
                DexNumber: int.Parse(parts[0][2..]),
                FormCode: form?[1..],
                // "pm133.cMay_2023" next to "pm133.cMAY_2023" - same costume, normalize the case
                CostumeCode: costume[1..].ToUpperInvariant()));
        }

        // .s / .g2 copies of the same sprite give identical records -> one each
        return result.Distinct().ToList();
    }

    // Upsert by the natural key (DexNumber, FormCode), never by Id:
    //   not in the DB yet           -> INSERT
    //   already in the DB           -> copy the fresh values onto the tracked row
    //                                  (EF sends an UPDATE only if a value actually changed)
    //   in the DB but not imported  -> left alone - never delete, listings will point at these rows
    private async Task SavePokemonAsync(List<Pokemon> imported)
    {
        // One query for the whole table (~1.3k rows) instead of one query per Pokemon
        var existing = await db.Pokemon.ToDictionaryAsync(p => (p.DexNumber, p.FormCode));
        var added = 0;

        foreach (var p in imported)
        {
            if (existing.TryGetValue((p.DexNumber, p.FormCode), out var row))
            {
                row.Name = p.Name;
                row.IsReleased = p.IsReleased;
                row.ShinyAvailable = p.ShinyAvailable;
                row.DynamaxAvailable = p.DynamaxAvailable;
                row.GigantamaxAvailable = p.GigantamaxAvailable;
            }
            else
            {
                db.Pokemon.Add(p);
                added++;
            }
        }

        // One transaction: either everything is saved, or nothing is.
        // Returns how many rows were written (INSERTs + UPDATEs).
        var written = await db.SaveChangesAsync();
        var updated = written - added;
        Console.WriteLine($"Saved: {added} added, {updated} updated, {imported.Count - written} unchanged");
    }

    private static async Task<List<GameMasterTemplate>> FetchTemplatesAsync()
    {
        // Stream the file instead of loading 19MB into one string first
        await using var stream = await Http.GetStreamAsync(GameMasterUrl);

        return await JsonSerializer.DeserializeAsync<List<GameMasterTemplate>>(stream, JsonOptions)
            ?? throw new Exception("Game master deserialized to null");
    }

    private static async Task<PogoApiLookups> FetchPogoApiAsync()
    {
        var names = await FetchPogoApiFileAsync("pokemon_names.json");
        var released = await FetchPogoApiFileAsync("released_pokemon.json");
        var shiny = await FetchPogoApiFileAsync("shiny_pokemon.json");

        return new PogoApiLookups(
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
    private static Pokemon ToPokemon(GameMasterForm form, PogoApiLookups pogo)
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
            DynamaxAvailable = form.DynamaxAvailable
                               && !MaxOverrides.NoDynamax.Contains((form.DexNumber, form.FormCode)),
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

        return $"{speciesName} ({TitleCase(formCode)})";
    }

    // Game code -> words for display: "WEST_SEA" -> "West Sea", "JAN_2020" -> "Jan 2020"
    private static string TitleCase(string code)
    {
        var words = code.Split('_').Select(w => w[0] + w[1..].ToLowerInvariant());
        return string.Join(' ', words);
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
    private static List<GameMasterForm> ParseForms(
        List<GameMasterTemplate> templates,
        Dictionary<(string Species, string? Form), (bool Dynamax, bool Gigantamax)> maxLookup)
    {
        var result = new List<GameMasterForm>();

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
                result.Add(new GameMasterForm(
                    dex, species.Key, "NORMAL", settings.IsTradable, max.Dynamax, max.Gigantamax));
                continue;
            }

            foreach (var (dex, settings) in formTemplates)
            {
                var formCode = StripSpeciesPrefix(settings.PokemonId, settings.Form!);
                var max = maxLookup.GetValueOrDefault((settings.PokemonId, settings.Form));
                result.Add(new GameMasterForm(
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
