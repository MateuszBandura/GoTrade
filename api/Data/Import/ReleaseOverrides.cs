namespace api.Data.Import;

// pogoapi.net released_pokemon.json is the base for Pokemon.IsReleased, but it stopped being
// updated (last change: 2026-01-31). This list fixes what it doesn't know about.
//
// Generated 2026-10-13 by comparing both sources: every species below is TRADABLE in the
// game master but NOT released according to pogoapi. Nothing disagrees the other way
// (pogoapi-released but missing from the game master = only untradable mythicals).
//
//   uncommented line = released in the game, pogoapi is out of date -> IsReleased = true
//   commented line   = really not in the game yet (game master is ahead) -> stays false
//
// New release in the game -> uncomment its line (or add a new dex number).
public static class ReleaseOverrides
{
    public static readonly HashSet<int> ReleasedSincePogoApi =
    [
        // 746,   // Wishiwashi
        // 771,   // Pyukumuku
        // 772,   // Type: Null
        // 773,   // Silvally
        774,   // Minior
        778,   // Mimikyu
        824,   // Blipbug
        825,   // Dottler
        826,   // Orbeetle
        // 833,   // Chewtle
        // 834,   // Drednaw
        843,   // Silicobra
        844,   // Sandaconda
        846,   // Arrokuda
        847,   // Barraskewda
        848,   // Toxel
        849,   // Toxtricity
        854,   // Sinistea
        // 868,   // Milcery
        // 869,   // Alcremie
        // 871,   // Pincurchin
        // 875,   // Eiscue
        // 878,   // Cufant
        // 879,   // Copperajah
        // 880,   // Dracozolt
        // 881,   // Arctozolt
        // 882,   // Dracovish
        // 883,   // Arctovish
        // 896,   // Glastrier
        // 897,   // Spectrier
        // 898,   // Calyrex
        931,   // Squawkabilly
        942,   // Maschiff
        943,   // Mabosstiff
        946,   // Bramblin
        947,   // Brambleghast
        // 951,   // Capsakid
        // 952,   // Scovillain
        // 953,   // Rellor
        // 954,   // Rabsca
        955,   // Flittle
        956,   // Espathra
        // 963,   // Finizen
        // 964,   // Palafin
        // 967,   // Cyclizar
        968,   // Orthworm
        973,   // Flamigo
        // 976,   // Veluza
        // 981,   // Farigiraf
        // 984,   // Great Tusk
        // 985,   // Scream Tail
        // 986,   // Brute Bonnet
        // 987,   // Flutter Mane
        // 988,   // Slither Wing
        // 989,   // Sandy Shocks
        // 990,   // Iron Treads
        // 991,   // Iron Bundle
        // 992,   // Iron Hands
        // 993,   // Iron Jugulis
        // 994,   // Iron Moth
        // 995,   // Iron Thorns
        // 1001,  // Wo-Chien
        // 1002,  // Chien-Pao
        // 1003,  // Ting-Lu
        // 1004,  // Chi-Yu
        // 1005,  // Roaring Moon
        // 1006,  // Iron Valiant
        // 1007,  // Koraidon
        // 1008,  // Miraidon
        // 1009,  // Walking Wake
        // 1010,  // Iron Leaves
        // 1014,  // Okidogi
        // 1015,  // Munkidori
        // 1016,  // Fezandipiti
        // 1017,  // Ogerpon
        // 1018,  // Archaludon
        // 1020,  // Gouging Fire
        // 1021,  // Raging Bolt
        // 1022,  // Iron Boulder
        // 1023,  // Iron Crown
        // 1024,  // Terapagos
        // 1025,  // Pecharunt
    ];
}
