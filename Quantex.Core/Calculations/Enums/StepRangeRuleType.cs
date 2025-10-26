using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<StepRangeRuleType>))]
public enum StepRangeRuleType
{
    FixedAmount,
    Percentage
}
