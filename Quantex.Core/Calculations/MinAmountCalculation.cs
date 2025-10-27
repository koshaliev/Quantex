using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который возвращает минимальное значение среди результатов вложенных (подчиненных) вычислений.
/// </summary>
public sealed class MinAmountCalculation : ICalculationMethod
{
    public List<ICalculationMethod> Calculations { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [];

    public MinAmountCalculation(List<ICalculationMethod> calculations)
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
