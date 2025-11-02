using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который возвращает заданную фиксированную сумму.
/// </summary>
[CalculationMethod("fixed")]
public sealed class FixedCalculation : ICalculationMethod
{
    public decimal Value { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [];

    public FixedCalculation(decimal value) => Value = value;

    public decimal Calculate(Dictionary<string, object> context) => Value;
}
