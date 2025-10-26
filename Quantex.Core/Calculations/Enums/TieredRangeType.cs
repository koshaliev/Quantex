using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations.Enums;


[JsonConverter(typeof(JsonStringEnumConverter<TieredRangeType>))]
public enum TieredRangeType
{
    Percentage,
    FixedAmount
}
