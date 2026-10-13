namespace api.Data.Import;

// Shape of the pogoapi.net files we read. Unlike the game master (an array),
// each file is ONE object keyed by dex number:
//   { "1": { "id": 1, "name": "Bulbasaur" }, "2": { ... }, ... }
// so it deserializes into Dictionary<int, PogoApiPokemon> - the JSON key becomes the dictionary key.
//
//   pokemon_names.json     every species        -> names
//   released_pokemon.json  released species     -> only the KEYS matter (is the dex in there?)
//   shiny_pokemon.json     species with shiny   -> only the KEYS matter
//                          (it has more fields - found_wild, found_raid... - we skip them)
public class PogoApiPokemon
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}

// The three files boiled down to what the importer needs
public record PogoApiData(
    Dictionary<int, string> Names,
    HashSet<int> Released,
    HashSet<int> Shiny);
