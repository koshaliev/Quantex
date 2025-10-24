using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Calculations;

public sealed class SumAmountCalculation : ICalculationMethod
{
    public List<ICalculationMethod> Calculations { get; }

    [JsonIgnore]
    public List<string> RequiredKeys { get; } = [];

    [JsonConstructor]
    public SumAmountCalculation(List<ICalculationMethod> calculations)
    {
        Calculations = calculations ?? throw new ArgumentNullException(nameof(calculations));
        if (Calculations.Count == 0)
            throw new ArgumentException("At least one calculation method must be provided.", nameof(calculations));

        for (int i = 0; i < Calculations.Count; i++)
        {
            if (Calculations[i] is null)
                throw new ArgumentException($"Calculation method at index {i} is null.", nameof(calculations));
            RequiredKeys.AddRange(Calculations[i].RequiredKeys);
        }
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (Calculations.Count == 0)
            return 0;
        var sum = 0m;
        for (int i = 0; i < Calculations.Count; i++)
        {
            sum += Calculations[i].Calculate(context);
        }
        return sum;
    }
}
