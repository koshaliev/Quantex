using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Calculations.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<StepRangeRuleType>))]
public enum StepRangeRuleType
{
    FixedAmount,
    Percentage
}
