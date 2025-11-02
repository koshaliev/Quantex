using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который выбирает вложенное вычисление, соответствующее шаговому диапазону на основе входных данных, и выполняет его, возвращая полученный результат.
/// </summary>
[CalculationMethod("universal-step")]
public sealed class UniversalStepRangeCalculation : ICalculationMethod
{
    public string Key { get; set; }
    public List<UniversalStepRangeRule> Ranges { get; }

    [JsonIgnore]
    private List<string>? _requiredKeys;

    [JsonIgnore]
    public List<string> RequiredKeys
    {
        get
        {
            if (_requiredKeys is null)
            {
                _requiredKeys = [];
                for (int i = 0; i < Ranges.Count; i++)
                {
                    for (int j = 0; j < Ranges[i].Calculation.RequiredKeys.Count; j++)
                        _requiredKeys.Add(Ranges[i].Calculation.RequiredKeys[j]);
                }
            }

            return _requiredKeys;
        }
    }

    public UniversalStepRangeCalculation(string key, List<UniversalStepRangeRule> ranges)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Ranges = ranges ?? throw new ArgumentNullException(nameof(ranges));
        ValidateAndSort(ranges);
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(UniversalStepRangeCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        for (int i = 0; i < Ranges.Count; i++)
        {
            if (IsInRange(Ranges[i], decimalValue))
                return Ranges[i].GetCost(context);
        }
        throw new ArgumentException($"No range found for value {decimalValue}");
    }

    private bool IsInRange(UniversalStepRangeRule range, decimal value) => value > range.From && value <= range.To;

    private static void ValidateAndSort(List<UniversalStepRangeRule> ranges)
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

public sealed class UniversalStepRangeRule
{
    public decimal From { get; init; }
    public decimal To { get; init; }
    public ICalculationMethod Calculation { get; init; }

    public UniversalStepRangeRule(decimal from, decimal to, ICalculationMethod calculation)
    {
        From = from;
        To = to;
        Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));
    }
    public decimal GetCost(Dictionary<string, object> context) => Calculation.Calculate(context);
    public override string ToString() => $"({From}..{To}] @ Calculation method: {Calculation}";
}
