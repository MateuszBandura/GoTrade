using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace api.Entities;

// Subclass of IdentityUser so we can add our own columns later.
// IdentityUser already gives us: Id, UserName, NormalizedUserName,
// Email, PasswordHash, etc. Add Pokemon Go specific fields here as
// the app grows, e.g.:
//   public string? TrainerCode { get; set; }   // 12-digit friend code
//   public int Level { get; set; }
//   public string? Team { get; set; }           // Mystic / Valor / Instinct
public class AppUser : IdentityUser
{
    
    [JsonIgnore]
    public Trainer Trainer { get; set; } = null!;
}
