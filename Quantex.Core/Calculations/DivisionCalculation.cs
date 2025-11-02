using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который делит значение из контекста на фиксированное число и возвращает итоговый результат.
/// </summary>
[CalculationMethod("/")]
public sealed class DivisionCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Value { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public DivisionCalculation(string key, decimal value)
    {
        Key = key;
        Value = value;
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(DivisionCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        return decimalValue / Value;
    }
}
