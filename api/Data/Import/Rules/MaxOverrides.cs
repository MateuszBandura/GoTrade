namespace api.Data.Import.Rules;

// The game master says what a form CAN do in Max Battles, not what was actually released:
// every Gigantamax form also has BREAD_MODE (Dynamax) there, even when Niantic never released
// its Dynamax version (Meowth). This list switches Dynamax off where the game master is ahead.
//
// Generated 2026-10-15: every tradable real form that has BREAD_MODE in its EXTENDED template.
//
//   commented line   = Dynamax version is in the game -> DynamaxAvailable stays true
//   uncommented line = NOT in the game                -> DynamaxAvailable = false
//
// [+Gmax] = the form also has Gigantamax - that one is NOT affected by this list.
// New Dynamax release -> comment its line out again.
public static class MaxOverrides
{
    public static readonly HashSet<(int Dex, string FormCode)> NoDynamax =
    [
        // (1, "NORMAL"),          // Bulbasaur
        // (2, "NORMAL"),          // Ivysaur
        // (3, "NORMAL"),          // Venusaur  [+Gmax]
        // (4, "NORMAL"),          // Charmander
        // (5, "NORMAL"),          // Charmeleon
        // (6, "NORMAL"),          // Charizard  [+Gmax]
        // (7, "NORMAL"),          // Squirtle
        // (8, "NORMAL"),          // Wartortle
        // (9, "NORMAL"),          // Blastoise  [+Gmax]
        // (10, "NORMAL"),         // Caterpie
        // (11, "NORMAL"),         // Metapod
        // (12, "NORMAL"),         // Butterfree  [+Gmax]
        // (25, "NORMAL"),         // Pikachu  [+Gmax]
        // (26, "NORMAL"),         // Raichu
        (52, "ALOLA"),          // Meowth (Alola)
        (52, "GALARIAN"),       // Meowth (Galarian)
        (52, "NORMAL"),         // Meowth  [+Gmax]
        // (58, "NORMAL"),         // Growlithe
        // (59, "NORMAL"),         // Arcanine
        // (63, "NORMAL"),         // Abra
        // (64, "NORMAL"),         // Kadabra
        // (65, "NORMAL"),         // Alakazam
        // (66, "NORMAL"),         // Machop
        // (67, "NORMAL"),         // Machoke
        // (68, "NORMAL"),         // Machamp  [+Gmax]
        // (92, "NORMAL"),         // Gastly
        // (93, "NORMAL"),         // Haunter
        // (94, "NORMAL"),         // Gengar  [+Gmax]
        // (98, "NORMAL"),         // Krabby
        // (99, "NORMAL"),         // Kingler  [+Gmax]
        // (106, "NORMAL"),        // Hitmonlee
        // (107, "NORMAL"),        // Hitmonchan
        // (111, "NORMAL"),        // Rhyhorn
        // (112, "NORMAL"),        // Rhydon
        // (113, "NORMAL"),        // Chansey
        (121, "NORMAL"),        // Starmie
        // (125, "NORMAL"),        // Electabuzz
        // (126, "NORMAL"),        // Magmar
        // (129, "NORMAL"),        // Magikarp
        // (130, "NORMAL"),        // Gyarados
        (131, "NORMAL"),        // Lapras  [+Gmax]
        // (133, "NORMAL"),        // Eevee
        // (134, "NORMAL"),        // Vaporeon
        // (135, "NORMAL"),        // Jolteon
        // (136, "NORMAL"),        // Flareon
        // (138, "NORMAL"),        // Omanyte
        // (139, "NORMAL"),        // Omastar
        // (140, "NORMAL"),        // Kabuto
        // (141, "NORMAL"),        // Kabutops
        (143, "NORMAL"),        // Snorlax  [+Gmax]
        // (144, "NORMAL"),        // Articuno
        // (145, "NORMAL"),        // Zapdos
        // (146, "NORMAL"),        // Moltres
        // (163, "NORMAL"),        // Hoothoot
        // (164, "NORMAL"),        // Noctowl
        // (196, "NORMAL"),        // Espeon
        // (197, "NORMAL"),        // Umbreon
        // (213, "NORMAL"),        // Shuckle
        // (215, "NORMAL"),        // Sneasel
        (236, "NORMAL"),        // Tyrogue
        // (237, "NORMAL"),        // Hitmontop
        (239, "NORMAL"),        // Elekid
        (240, "NORMAL"),        // Magby
        // (242, "NORMAL"),        // Blissey
        // (243, "NORMAL"),        // Raikou
        // (244, "NORMAL"),        // Entei
        // (245, "NORMAL"),        // Suicune
        // (249, "NORMAL"),        // Lugia
        // (250, "NORMAL"),        // Ho-Oh
        // (280, "NORMAL"),        // Ralts
        // (281, "NORMAL"),        // Kirlia
        // (282, "NORMAL"),        // Gardevoir
        // (302, "NORMAL"),        // Sableye
        // (320, "NORMAL"),        // Wailmer
        // (321, "NORMAL"),        // Wailord
        // (328, "NORMAL"),        // Trapinch
        // (329, "NORMAL"),        // Vibrava
        // (330, "NORMAL"),        // Flygon
        // (349, "NORMAL"),        // Feebas
        // (350, "NORMAL"),        // Milotic
        // (363, "NORMAL"),        // Spheal
        // (364, "NORMAL"),        // Sealeo
        // (365, "NORMAL"),        // Walrein
        // (374, "NORMAL"),        // Beldum
        // (375, "NORMAL"),        // Metang
        // (376, "NORMAL"),        // Metagross
        // (377, "NORMAL"),        // Regirock
        // (378, "NORMAL"),        // Regice
        // (379, "NORMAL"),        // Registeel
        // (380, "NORMAL"),        // Latias
        // (381, "NORMAL"),        // Latios
        // (415, "NORMAL"),        // Combee
        // (416, "NORMAL"),        // Vespiquen
        // (461, "NORMAL"),        // Weavile
        // (464, "NORMAL"),        // Rhyperior
        // (466, "NORMAL"),        // Electivire
        // (467, "NORMAL"),        // Magmortar
        // (470, "NORMAL"),        // Leafeon
        // (471, "NORMAL"),        // Glaceon
        // (475, "NORMAL"),        // Gallade
        // (519, "NORMAL"),        // Pidove
        // (520, "NORMAL"),        // Tranquill
        // (521, "NORMAL"),        // Unfezant
        // (524, "NORMAL"),        // Roggenrola
        // (525, "NORMAL"),        // Boldore
        // (526, "NORMAL"),        // Gigalith
        // (527, "NORMAL"),        // Woobat
        // (528, "NORMAL"),        // Swoobat
        // (529, "NORMAL"),        // Drilbur
        // (530, "NORMAL"),        // Excadrill
        // (546, "NORMAL"),        // Cottonee
        // (547, "NORMAL"),        // Whimsicott
        // (554, "NORMAL"),        // Darumaka
        // (555, "STANDARD"),      // Darmanitan (Standard)
        // (568, "NORMAL"),        // Trubbish
        // (569, "NORMAL"),        // Garbodor  [+Gmax]
        // (615, "NORMAL"),        // Cryogonal
        // (633, "NORMAL"),        // Deino
        // (634, "NORMAL"),        // Zweilous
        // (635, "NORMAL"),        // Hydreigon
        (677, "NORMAL"),        // Espurr
        (678, "FEMALE"),        // Meowstic (Female)
        (678, "NORMAL"),        // Meowstic
        // (686, "NORMAL"),        // Inkay
        // (687, "NORMAL"),        // Malamar
        // (700, "NORMAL"),        // Sylveon
        // (761, "NORMAL"),        // Bounsweet
        // (762, "NORMAL"),        // Steenee
        // (763, "NORMAL"),        // Tsareena
        // (766, "NORMAL"),        // Passimian
        // (780, "NORMAL"),        // Drampa
        // (810, "NORMAL"),        // Grookey
        // (811, "NORMAL"),        // Thwackey
        // (812, "NORMAL"),        // Rillaboom  [+Gmax]
        // (813, "NORMAL"),        // Scorbunny
        // (814, "NORMAL"),        // Raboot
        // (815, "NORMAL"),        // Cinderace  [+Gmax]
        // (816, "NORMAL"),        // Sobble
        // (817, "NORMAL"),        // Drizzile
        // (818, "NORMAL"),        // Inteleon  [+Gmax]
        // (819, "NORMAL"),        // Skwovet
        // (820, "NORMAL"),        // Greedent
        // (821, "NORMAL"),        // Rookidee
        // (822, "NORMAL"),        // Corvisquire
        // (823, "NORMAL"),        // Corviknight
        // (831, "NORMAL"),        // Wooloo
        // (832, "NORMAL"),        // Dubwool
        (848, "NORMAL"),        // Toxel
        // (849, "AMPED"),         // Toxtricity (Amped)  [+Gmax]
        // (849, "LOW_KEY"),       // Toxtricity (Low Key)  [+Gmax]
        // (850, "NORMAL"),        // Sizzlipede
        // (851, "NORMAL"),        // Centiskorch
        // (856, "NORMAL"),        // Hatenna
        // (857, "NORMAL"),        // Hattrem
        // (858, "NORMAL"),        // Hatterene
        (861, "NORMAL"),        // Grimmsnarl  [+Gmax]
        // (870, "NORMAL"),        // Falinks
        // (884, "NORMAL"),        // Duraludon
        // (891, "NORMAL"),        // Kubfu
        // (892, "RAPID_STRIKE"),  // Urshifu (Rapid Strike)  [+Gmax]
        // (892, "SINGLE_STRIKE"), // Urshifu (Single Strike)  [+Gmax]
    ];
}
