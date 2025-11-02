using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который возвращает сумму, составляющую указанный процент от значения, содержащегося в контексте.
/// </summary>
[CalculationMethod("percentage")]
public sealed class PercentageCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Value { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public PercentageCalculation(string key, decimal value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(PercentageCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        return decimalValue / 100m * Value;
    }
}
