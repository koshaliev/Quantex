using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Calculations.Enums;


[JsonConverter(typeof(JsonStringEnumConverter<TieredRangeType>))]
public enum TieredRangeType
{
    Percentage,
    FixedAmount
}
