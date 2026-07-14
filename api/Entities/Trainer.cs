using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api.Enums;

namespace api.Entities;

public class Trainer
{
    public string Id { get; set; } = null!;

    [Required]
    public string FriendCode { get; set; } = null!;
    
    public Team? Team { get; set; } = null;

    public int? Level { get; set; } = null;

    public string? Region { get; set; } = null;
    
    [JsonIgnore]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public AppUser User { get; set; } = null!;
}
