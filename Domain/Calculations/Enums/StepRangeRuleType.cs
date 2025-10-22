using System.Text.Json.Serialization;

namespace Quantex.Domain.Calculations.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<StepRangeRuleType>))]
public enum StepRangeRuleType
{
    FixedAmount,
    Percentage
}
