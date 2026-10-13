namespace api.Data.Import;

// One (species, form) pair pulled out of a pokemonSettings template,
// plus whatever we learn about it from other templates.
// Middle step between the JSON shape and the Pokemon entity - still nothing to do with the DB.
//   DexNumber            52
//   SpeciesCode          MEOWTH
//   FormCode             ALOLA
//   IsTradable           true
//   DynamaxAvailable     true
//   GigantamaxAvailable  false
public record ParsedForm(
    int DexNumber,
    string SpeciesCode,
    string FormCode,
    bool IsTradable,
    bool DynamaxAvailable,
    bool GigantamaxAvailable);
