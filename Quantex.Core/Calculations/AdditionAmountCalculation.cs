using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который прибавляет фиксированное значение к значению из контекста и возвращает итоговый результат.
/// </summary>
public sealed class AdditionAmountCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Amount { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public AdditionAmountCalculation(string key, decimal amount)
    {
        Key = key;
        Amount = amount;
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(AdditionAmountCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        return decimalValue + Amount;
    }
}
