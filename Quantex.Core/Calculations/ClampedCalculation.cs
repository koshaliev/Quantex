using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

public sealed class ClampedCalculation : ICalculationMethod
{
    public ICalculationMethod Calculation { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => Calculation.RequiredKeys;

    public ClampedCalculation(ICalculationMethod calculation, decimal? minAmount = null, decimal? maxAmount = null)
    {
        Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));
        MinAmount = minAmount;
        MaxAmount = maxAmount;
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        var result = Calculation.Calculate(context);

        if (MinAmount is decimal min && result < min)
            return min;

        if (MaxAmount is decimal max && result > max)
            return max;

        return result;
    }
}
