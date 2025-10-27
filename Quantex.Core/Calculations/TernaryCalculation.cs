﻿using System.Text.Json.Serialization;
using Quantex.Core.Conditions;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который выбирает и выполняет одно из двух вычислений в зависимости от результата проверки заданного условия.
/// </summary>
public sealed class TernaryCalculation : ICalculationMethod
{
    public ICondition Condition { get; }
    public ICalculationMethod IfTrue { get; }
    public ICalculationMethod IfFalse { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => [];

    public TernaryCalculation(ICondition condition, ICalculationMethod ifTrue, ICalculationMethod ifFalse)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        IfTrue = ifTrue ?? throw new ArgumentNullException(nameof(ifTrue));
        IfFalse = ifFalse ?? throw new ArgumentNullException(nameof(ifFalse));

        for (int i = 0; i < Condition.RequiredKeys.Count; i++)
            RequiredKeys.Add(Condition.RequiredKeys[i]);

        for (int i = 0; i < IfTrue.RequiredKeys.Count; i++)
            RequiredKeys.Add(IfTrue.RequiredKeys[i]);

        for (int i = 0; i < IfTrue.RequiredKeys.Count; i++)
            RequiredKeys.Add(IfFalse.RequiredKeys[i]);
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        return Condition.IsSatisfied(context)
            ? IfTrue.Calculate(context)
            : IfFalse.Calculate(context);
    }
}
