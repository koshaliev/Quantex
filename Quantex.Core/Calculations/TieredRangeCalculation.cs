using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который вычисляет результат путём суммирования значений по соответствующим диапазонам (уровням).
/// </summary>
public sealed class TieredRangeCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public IReadOnlyList<TieredRangeRule> Ranges { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public TieredRangeCalculation(string key, List<TieredRangeRule> ranges)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(ranges);

        Key = key ?? throw new ArgumentNullException(nameof(key));
        Ranges = ranges ?? throw new ArgumentNullException(nameof(ranges));
        ValidateAndSort(ranges);
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(TieredRangeCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        decimal totalCost = 0;
        for (int i = 0; i < Ranges.Count; i++)
        {
            if (Ranges[i].TryGetCost(decimalValue, out var cost))
                totalCost += cost;
        }
        return totalCost;
    }

    private static void ValidateAndSort(List<TieredRangeRule> ranges)
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
                throw new ArgumentException("Ranges must be continuous and non-overlapping");
        }
    }
}

public class TieredRangeRule
{
    public decimal From { get; init; }
    public decimal To { get; init; }
    public decimal Value { get; init; }
    public TieredRangeType Type { get; init; }

    public TieredRangeRule(decimal from, decimal to, decimal value, TieredRangeType type)
    {
        From = from;
        To = to;
        Value = value;
        Type = type;
    }

    public bool IsInRange(decimal amount) => amount >= From;

    public bool TryGetCost(decimal amount, out decimal cost)
    {
        cost = 0;
        if (!IsInRange(amount))
            return false;

        cost = Type switch
        {
            TieredRangeType.Fixed => Value,
            TieredRangeType.Percentage => (Math.Min(amount, To) - From) / 100m * Value,
            _ => throw new NotSupportedException($"Unsupported TieredRangeType: {Type}")
        };
        return true;
    }

    public override string ToString() => $"[{From}..{To}) @ {Type}: {Value}{(Type == TieredRangeType.Percentage ? " %" : string.Empty)}";
}

[JsonConverter(typeof(JsonStringEnumConverter<TieredRangeType>))]
public enum TieredRangeType
{
    Fixed,
    Percentage
}
