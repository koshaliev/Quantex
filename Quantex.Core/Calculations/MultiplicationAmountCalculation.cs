using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

public sealed class MultiplicationAmountCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Multiplier { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public MultiplicationAmountCalculation(string key, decimal multiplier)
    {
        Key = key;
        Multiplier = multiplier;
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(MultiplicationAmountCalculation)}. Key '{Key}' not found or Value is not decimal in context.");


        return decimalValue * Multiplier;
    }
}
