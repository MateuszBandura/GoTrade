using System.Text.RegularExpressions;

namespace api.Data.Import;

public enum FormBucket { RealForm, Costume, Skip }

// Decides what the importer does with each (species, form) pair from the FORMS templates.
//   RealForm -> own Pokemon row
//   Costume  -> Costume + PokemonCostume (SpriteCode = F)
//   Skip     -> nothing
//
// Default is RealForm - only the EXCEPTIONS are listed here.
// Keys are FULL names (SPECIES_FORM), because the bare code is ambiguous:
// UNOWN_S is the letter S, LUGIA_S is a shadow variant.
// Untradable forms (mythicals, fusions) are NOT listed - the importer drops them via isTradable.
public static class FormClassification
{
    // Costumes are recognised by rule (see IsCostume). These are the ones the rule misses.
    private static readonly HashSet<string> CostumesWithoutYear =
    [
        "GIMMIGHOUL_COIN_A1",
    ];

    private static readonly HashSet<string> Skip =
    [
        // Shadow-only variants - shadows can't be traded
        "RAIKOU_S", "ENTEI_S", "SUICUNE_S", "LUGIA_S", "HO_OH_S", "LATIAS_S", "LATIOS_S",

        // Battle-only states - you can't catch or keep a Pokemon in this form
        "AEGISLASH_BLADE",
        "MIMIKYU_BUSTED",
        "CRAMORANT_GULPING", "CRAMORANT_GORGING",
        "MORPEKO_HANGRY",
        "DARMANITAN_ZEN", "DARMANITAN_GALARIAN_ZEN",
        "WISHIWASHI_SCHOOL",
        "EISCUE_NOICE",

        // Sizes - not trade-relevant. AVERAGE stays as the one row (and its sprite exists).
        "PUMPKABOO_SMALL", "PUMPKABOO_LARGE", "PUMPKABOO_SUPER",
        "GOURGEIST_SMALL", "GOURGEIST_LARGE", "GOURGEIST_SUPER",
    ];

    public static FormBucket Classify(ParsedForm form)
    {
        var fullName = $"{form.SpeciesCode}_{form.FormCode}";

        if (Skip.Contains(fullName)) return FormBucket.Skip;
        if (IsCostume(form, fullName)) return FormBucket.Costume;
        return FormBucket.RealForm;
    }

    private static bool IsCostume(ParsedForm form, string fullName)
    {
        if (form.FormCode == "NORMAL") return false;

        return form.SpeciesCode == "PIKACHU"                 // every non-NORMAL Pikachu "form" is a costume
            || Regex.IsMatch(form.FormCode, @"20\d\d")       // a year = event costume: FALL_2019, WINTER_2025, SLOWPOKE_2020
            || CostumesWithoutYear.Contains(fullName);
    }
}
