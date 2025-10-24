using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Calculations;

public sealed class PercentageCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Percentage { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    [JsonConstructor]
    public PercentageCalculation(string key, decimal percentage)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Percentage = percentage;
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(PercentageCalculation)}. Key \"{Key}\" not found");

        return decimalValue / 100m * Percentage;
    }
}
