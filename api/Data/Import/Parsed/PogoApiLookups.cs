namespace api.Data.Import.Parsed;

// The three pogoapi files boiled down to what the importer needs
public record PogoApiLookups(
    Dictionary<int, string> Names,
    HashSet<int> Released,
    HashSet<int> Shiny);
