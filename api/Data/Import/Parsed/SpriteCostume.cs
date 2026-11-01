namespace api.Data.Import.Parsed;

// One (species, form, costume) combination parsed from a sprite file name:
//   pm77.fGALARIAN.cGOFEST_2021_NOEVOLVE.s.icon.png -> (77, "GALARIAN", "GOFEST_2021_NOEVOLVE")
//   pm1.cJAN_2020_NOEVOLVE.icon.png                 -> (1,  null,       "JAN_2020_NOEVOLVE")
// A record compares by value, so Distinct() collapses the shiny / female copies of the same sprite.
public record SpriteCostume(int DexNumber, string? FormCode, string CostumeCode);
