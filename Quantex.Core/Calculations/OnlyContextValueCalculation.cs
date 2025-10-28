namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, возвращающий значение из контекста.
/// </summary>
public sealed class OnlyContextValueCalculation : ICalculationMethod
{
    public string Key { get; init; }
    public List<string> RequiredKeys => [Key];

    public OnlyContextValueCalculation(string key)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(ContextValueAdditionCalculation)}. Key '{Key}' not found or Value is not decimal in context.");
        return decimalValue;
    }
}
