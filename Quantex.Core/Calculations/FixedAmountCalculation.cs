using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который возвращает заданную фиксированную сумму.
/// </summary>
public sealed class FixedAmountCalculation : ICalculationMethod
{
    public decimal Amount { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [];

    public FixedAmountCalculation(decimal amount) => Amount = amount;

    public decimal Calculate(Dictionary<string, object> context) => Amount;
}
