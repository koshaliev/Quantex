using System;
using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который выбирает значение из шагового диапазона на основе входного значения.
/// </summary>
public sealed class StepRangeCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public IReadOnlyList<StepRangeRule> Ranges { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public StepRangeCalculation(string key, List<StepRangeRule> ranges)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Ranges = ranges ?? throw new ArgumentNullException(nameof(ranges));
        ValidateAndSort(ranges);
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(StepRangeCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        for (int i = 0; i < Ranges.Count; i++)
        {
            if (Ranges[i].TryGetCost(decimalValue, out var cost))
                return cost;
        }
        throw new ArgumentException($"No range found for value {decimalValue}");
    }

    private static void ValidateAndSort(List<StepRangeRule> ranges)
    {
        if (ranges.Count == 0)
            throw new ArgumentException("Ranges list cannot be empty", nameof(ranges));

        ranges.Sort((a, b) => a.From.CompareTo(b.From));

        for (int i = 0; i < ranges.Count; i++)
        {
            if (ranges[i].From >= ranges[i].To)
                throw new ArgumentException($"Range {i}: From ({ranges[i].From}) must be less than To ({ranges[i].To})");
        }

        for (int i = 1; i < ranges.Count; i++)
        {
            if (ranges[i].From != ranges[i - 1].To)
                throw new ArgumentException($"Ranges must be continuous and non-overlapping. Problem in range with index = {i}: {ranges[i]}");
        }
    }
}

public sealed class StepRangeRule
{
    public decimal From { get; init; }
    public decimal To { get; init; }
    public decimal Value { get; init; }
    public StepRangeRuleType Type { get; init; }

    public StepRangeRule(decimal from, decimal to, decimal value, StepRangeRuleType type)
    {
        From = from;
        To = to;
        Value = value;
        
        if (!Enum.IsDefined(type))
            throw new ArgumentException($"Invalid enum value for {typeof(StepRangeRuleType).Name}.");
        Type = type;
    }

    public bool IsInRange(decimal amount) => amount >= From && amount < To;
    public bool TryGetCost(decimal amount, out decimal cost)
    {
        cost = 0;
        if (!IsInRange(amount))
            return false;

        cost = Type switch // TODO: may be strategy pattern again?
        {
            StepRangeRuleType.FixedAmount => Value,
            StepRangeRuleType.Percentage => amount / 100m * Value,
            _ => throw new NotSupportedException($"Unsupported StepRangeRuleType: {Type}")
        };
        return true;
    }

    public override string ToString() => $"[{From}..{To}) @ {Type}: {Value}{(Type == StepRangeRuleType.Percentage ? "%" : string.Empty)}";
}

[JsonConverter(typeof(JsonStringEnumConverter<StepRangeRuleType>))]
public enum StepRangeRuleType
{
    FixedAmount,
    Percentage
}
