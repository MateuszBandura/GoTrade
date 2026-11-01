namespace api.Data.Import.Json;

// GitHub "git trees" API response for one folder of PokeMiners/pogo_assets:
//   { "tree": [ { "path": "pm1.cJAN_2020_NOEVOLVE.icon.png", "type": "blob", ... }, ... ],
//     "truncated": false }
// We only need the file names. The game master has no "which species wears which costume"
// list - these file names are the only source for regular (.c) costumes.
public class GitHubTree
{
    public List<GitHubTreeEntry> Tree { get; set; } = [];

    // true = GitHub cut the list short (limit is far above the ~3.5k files we get)
    public bool Truncated { get; set; }
}

public class GitHubTreeEntry
{
    public string Path { get; set; } = null!;
}
