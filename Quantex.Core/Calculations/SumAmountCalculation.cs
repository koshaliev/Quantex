using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который суммирует результаты, полученные от нескольких вложенных (подчиненных) вычислений.
/// </summary>
public sealed class SumAmountCalculation : ICalculationMethod
{
    public List<ICalculationMethod> Calculations { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [];

    public SumAmountCalculation(List<ICalculationMethod> calculations)
    {
        Calculations = calculations ?? throw new ArgumentNullException(nameof(calculations));
        if (Calculations.Count == 0)
            throw new ArgumentException("At least one calculation method must be provided.", nameof(calculations));

        for (int i = 0; i < Calculations.Count; i++)
        {
            ArgumentNullException.ThrowIfNull(Calculations[i]);
            for (int j = 0; j < Calculations[i].RequiredKeys.Count; j++)
                RequiredKeys.Add(Calculations[i].RequiredKeys[j]);
        }
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (Calculations.Count == 0)
            throw new ArgumentException("At least one calculation method must be provided.", nameof(Calculations));

        var sum = 0m;
        for (int i = 0; i < Calculations.Count; i++)
            sum += Calculations[i].Calculate(context);
        return sum;
    }
}
