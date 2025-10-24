using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Calculations;

public sealed class UniversalTieredRangeCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public IReadOnlyList<UniversalTieredRangeRule> Ranges { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

#nullable disable
    [JsonConstructor]
    private UniversalTieredRangeCalculation() { }
#nullable enable

    public UniversalTieredRangeCalculation(string key, List<UniversalTieredRangeRule> ranges)
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
            throw new ArgumentException($"Invalid context for {nameof(TieredRangeCalculation)}. Key \"{Key}\" not found or Value is not decimal");

        decimal totalCost = 0;
        for (int i = 0; i < Ranges.Count; i++)
        {
            if (IsInRange(Ranges[i], decimalValue))
            {
                totalCost += Ranges[i].GetCost(context);
            }
        }

        return totalCost;
    }

    private bool IsInRange(UniversalTieredRangeRule range, decimal value) => value >= range.From && value < range.To;

    private static void ValidateAndSort(List<UniversalTieredRangeRule> ranges)
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
            {
                throw new ArgumentException("Ranges must be continuous and non-overlapping");
            }
        }
    }
}

public sealed class UniversalTieredRangeRule
{
    public decimal From { get; init; }
    public decimal To { get; init; }
    public ICalculationMethod Calculation { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => Calculation.RequiredKeys;

#nullable disable
    [JsonConstructor]
    private UniversalTieredRangeRule() { }
#nullable enable

    public UniversalTieredRangeRule(decimal from, decimal to, ICalculationMethod calculation)
    {
        From = from;
        To = to;
        Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));
    }

    public decimal GetCost(Dictionary<string, object> context) => Calculation.Calculate(context);
    public override string ToString() => $"[{From}..{To}) @ Calculation method: {Calculation}";
}
