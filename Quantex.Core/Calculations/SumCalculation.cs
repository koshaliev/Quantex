using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который суммирует результаты, полученные от нескольких вложенных (подчиненных) вычислений.
/// </summary>
public sealed class SumCalculation : ICalculationMethod
{
    public List<ICalculationMethod> Calculations { get; init; }

    [JsonIgnore]
    private List<string>? _requiredKeys;

    [JsonIgnore]
    public List<string> RequiredKeys
    {
        get
        {
            if (_requiredKeys is null)
            {
                _requiredKeys = [];
                for (int i = 0; i < Calculations.Count; i++)
                {
                    for (int j = 0; j < Calculations[i].RequiredKeys.Count; j++)
                        _requiredKeys.Add(Calculations[i].RequiredKeys[j]);
                }
            }
            return _requiredKeys;
        }
    }

    public SumCalculation(List<ICalculationMethod> calculations)
    {
        Calculations = calculations ?? throw new ArgumentNullException(nameof(calculations));
        if (Calculations.Count == 0)
            throw new ArgumentException("At least one calculation method must be provided.", nameof(calculations));
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
