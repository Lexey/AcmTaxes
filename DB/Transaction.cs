using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Acm.DB;

public enum BuySell
{
    Buy,
    Sell
}

[JsonObject(MemberSerialization.OptIn)]
public record Transaction
{
    [JsonProperty]
    public required DateTime SettlementDate { get; set; }

    [JsonProperty]
    public required string Id { get; set; }

    [JsonProperty]
    [JsonConverter(typeof(StringEnumConverter))]
    public required BuySell Type { get; set; }

    [JsonProperty]
    public required string AssetType { get; set; }

    [JsonProperty]
    public required string Asset { get; set; }

    [JsonProperty]
    public required string Ticker { get; set; }

    [JsonProperty]
    public required string ISIN { get; set; }

    [JsonProperty]
    public required int Quantity { get; set; }

    [JsonProperty]
    public required decimal Price { get; set; }
    
    [JsonProperty]
    public required decimal Total { get; set; }
    
    [JsonProperty]
    public required string Currency { get; set; }
    
    [JsonProperty]
    public required int SourceIndex { get; set; }
}
