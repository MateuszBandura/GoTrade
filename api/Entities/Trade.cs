using System.Text.Json.Serialization;
using api.Enums;

namespace api.Entities;

// A trade between two trainers, and the conversation around it (Messages).
// Listings are optional: trainers can start talking before they agree on what they exchange.
// The service requires both listings before Completed, and blocks editing a listing used in a trade -
// that's what keeps the history true without copying Pokemon data here.
public class Trade
{
    public int Id { get; set; }

    public string InitiatorId { get; set; } = null!;

    public string ResponderId { get; set; } = null!;

    public int? InitiatorListingId { get; set; }

    public int? ResponderListingId { get; set; }

    public TradeStatus Status { get; set; } = TradeStatus.Proposed;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Trainer Initiator { get; set; } = null!;

    [JsonIgnore]
    public Trainer Responder { get; set; } = null!;

    [JsonIgnore]
    public TradeListing? InitiatorListing { get; set; }

    [JsonIgnore]
    public TradeListing? ResponderListing { get; set; }

    [JsonIgnore]
    public List<TradeMessage> Messages { get; set; } = [];
}
