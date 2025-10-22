using System.Text.Json.Serialization;

namespace Quantex.Domain.Calculations;

public sealed class MinAmountCalculation : ICalculationMethod
{
    public List<ICalculationMethod> Calculations { get; }

    [JsonIgnore]
    public List<string> RequiredKeys { get; } = [];

    [JsonConstructor]
    public MinAmountCalculation(List<ICalculationMethod> calculations)
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
        var min = Calculations[0].Calculate(context);
        for (int i = 1; i < Calculations.Count; i++)
        {
            var value = Calculations[i].Calculate(context);
            if (value < min)
                min = value;
        }
        return min;
    }
}
