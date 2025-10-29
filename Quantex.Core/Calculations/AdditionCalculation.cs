using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который прибавляет фиксированное значение к значению из контекста и возвращает итоговый результат.
/// </summary>
public sealed class AdditionCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public decimal Value { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public AdditionCalculation(string key, decimal value)
    {
        Key = key;
        Value = value;
    }
    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(AdditionCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        return decimalValue + Value;
    }
}
