using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

public sealed class OppositeAmountCalculation : ICalculationMethod
{
    public ICalculationMethod Calculation { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => Calculation.RequiredKeys;

    [JsonConstructor]
    public OppositeAmountCalculation(ICalculationMethod calculation)
    {
        Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));
    }

    public decimal Calculate(Dictionary<string, object> context) => -Calculation.Calculate(context);
}
