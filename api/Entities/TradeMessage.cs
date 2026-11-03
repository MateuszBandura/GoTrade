using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Entities;

// The service checks that the sender takes part in the trade (a CHECK can't look into another table)
public class TradeMessage
{
    public int Id { get; set; }

    public int TradeId { get; set; }

    public string SenderId { get; set; } = null!;

    [Required]
    public string Body { get; set; } = null!;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    // null = not read yet
    public DateTime? ReadAt { get; set; }

    [JsonIgnore]
    public Trade Trade { get; set; } = null!;

    [JsonIgnore]
    public Trainer Sender { get; set; } = null!;
}
