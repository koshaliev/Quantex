using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который возвращает сумму, составляющую указанный процент от значения, содержащегося в контексте.
/// </summary>
public sealed class PercentageCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Percentage { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public PercentageCalculation(string key, decimal percentage)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Percentage = percentage;
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(PercentageCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        return decimalValue / 100m * Percentage;
    }
}
