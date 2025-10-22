using System.Text.Json.Serialization;
using Quantex.Domain.Conditions;

namespace Quantex.Domain.Calculations;

public sealed class TernaryCalculation : ICalculationMethod
{
    public ICondition Condition { get; }
    public ICalculationMethod IfTrue { get; }
    public ICalculationMethod IfFalse { get; }

    [JsonIgnore]
    public List<string> RequiredKeys { get; } = [];

    [JsonConstructor]
    public TernaryCalculation(ICondition condition, ICalculationMethod ifTrue, ICalculationMethod ifFalse)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        IfTrue = ifTrue ?? throw new ArgumentNullException(nameof(ifTrue));
        IfFalse = ifFalse ?? throw new ArgumentNullException(nameof(ifFalse));
        RequiredKeys.AddRange(Condition.RequiredKeys);
        RequiredKeys.AddRange(IfTrue.RequiredKeys);
        RequiredKeys.AddRange(IfFalse.RequiredKeys);
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        return Condition.IsSatisfied(context)
            ? IfTrue.Calculate(context)
            : IfFalse.Calculate(context);
    }
}
