using System.Text.Json.Serialization;

namespace Quantex.Domain.Calculations.Enums;


[JsonConverter(typeof(JsonStringEnumConverter<TieredRangeType>))]
public enum TieredRangeType
{
    Percentage,
    FixedAmount
}
