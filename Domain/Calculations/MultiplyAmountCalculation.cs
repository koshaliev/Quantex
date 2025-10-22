using System.Text.Json.Serialization;

namespace Quantex.Domain.Calculations;

public sealed class MultiplyAmountCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Multiplier { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    [JsonConstructor]
    public MultiplyAmountCalculation(string key, decimal multiplier)
    {
        Key = key;
        Multiplier = multiplier;
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
        {
            throw new ArgumentException($"Key '{Key}' not found or is not a decimal in the context.");
        }
        return decimalValue * Multiplier;
    }
}
