using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Calculations;

public sealed class AdditionAmountCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Amount { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    [JsonConstructor]
    public AdditionAmountCalculation(string key, decimal addend)
    {
        Key = key;
        Amount = addend;
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
        {
            throw new ArgumentException($"Key '{Key}' not found or is not a decimal in the context.");
        }
        return decimalValue + Amount;
    }
}
