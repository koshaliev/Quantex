using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

public sealed class ContextValueMultiplicationCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Amount { get; init; }
    public ICalculationMethod Calculation { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => Calculation.RequiredKeys;

    public ContextValueMultiplicationCalculation(string key, decimal amount, ICalculationMethod calculation)
    {
        Key = key;
        Amount = amount;
        Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));

        for (int i = 0; i < Calculation.RequiredKeys.Count; i++)
        {
            RequiredKeys.Add(Calculation.RequiredKeys[i]);
        }
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(ContextValueMultiplicationCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        var originalValue = decimalValue;
        context[Key] = decimalValue * Amount;
        var amount = Calculation.Calculate(context);
        context[Key] = originalValue;

        return amount;
    }
}