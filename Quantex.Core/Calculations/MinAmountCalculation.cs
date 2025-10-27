using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

public sealed class MinAmountCalculation : ICalculationMethod
{
    [JsonIgnore]
    private List<string>? _requiredKeys;

    public List<ICalculationMethod> Calculations { get; init; }

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
                    {
                        _requiredKeys.Add(Calculations[i].RequiredKeys[j]);
                    }
                }
            }
            return _requiredKeys;
        }
    }

    public MinAmountCalculation(List<ICalculationMethod> calculations)
    {
        Calculations = calculations ?? throw new ArgumentNullException(nameof(calculations));
        if (Calculations.Count == 0)
            throw new ArgumentException("At least one calculation method must be provided.", nameof(calculations));
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
