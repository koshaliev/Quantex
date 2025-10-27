using System.Text.Json.Serialization;
using Quantex.Core.Conditions;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который выбирает и выполняет одно из двух вычислений в зависимости от результата проверки заданного условия.
/// </summary>
public sealed class TernaryCalculation : ICalculationMethod
{
    [JsonIgnore]
    private List<string>? _requiredKeys;

    public ICondition Condition { get; }
    public ICalculationMethod IfTrue { get; }
    public ICalculationMethod IfFalse { get; }

    [JsonIgnore]
    public List<string> RequiredKeys
    {
        get
        {
            if (_requiredKeys is null)
            {
                _requiredKeys = [];
                for (int i = 0; i < Condition.RequiredKeys.Count; i++)
                    _requiredKeys.Add(Condition.RequiredKeys[i]);

                for (int i = 0; i < IfTrue.RequiredKeys.Count; i++)
                    _requiredKeys.Add(IfTrue.RequiredKeys[i]);

                for (int i = 0; i < IfTrue.RequiredKeys.Count; i++)
                    _requiredKeys.Add(IfFalse.RequiredKeys[i]);
            }
            return _requiredKeys;
        }
    }

    public TernaryCalculation(ICondition condition, ICalculationMethod ifTrue, ICalculationMethod ifFalse)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        IfTrue = ifTrue ?? throw new ArgumentNullException(nameof(ifTrue));
        IfFalse = ifFalse ?? throw new ArgumentNullException(nameof(ifFalse));
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        return Condition.IsSatisfied(context)
            ? IfTrue.Calculate(context)
            : IfFalse.Calculate(context);
    }
}
