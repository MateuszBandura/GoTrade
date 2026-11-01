using api.Enums;

namespace api.Data.Import.Parsed;

// "This Pokemon row exists in this costume" - one future PokemonCostume row, before the DB.
// Points at the Pokemon by its natural key (DexNumber, FormCode): the Ids only exist after SAVE.
//   (25, "NORMAL",   "WCS_2022",             F)   <- game master form PIKACHU_WCS_2022
//   (77, "GALARIAN", "GOFEST_2021_NOEVOLVE", C)   <- sprite pm77.fGALARIAN.cGOFEST_2021_NOEVOLVE
public record CostumeLink(int DexNumber, string FormCode, string CostumeCode, SpriteCode SpriteCode);
