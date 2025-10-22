using System.Text.Json.Serialization;

namespace Quantex.Domain.Calculations;

public sealed class FixedAmountCalculation : ICalculationMethod
{
    public decimal Amount { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [];

    [JsonConstructor]
    public FixedAmountCalculation(decimal amount) => Amount = amount;

    public decimal Calculate(Dictionary<string, object> context) => Amount;
}
