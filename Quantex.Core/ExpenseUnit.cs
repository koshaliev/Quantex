using System.Text.Json.Serialization;
using Quantex.Core.Calculations;
using Quantex.Core.Conditions;

namespace Quantex.Core;

/// <summary>
/// Единица расхода. 
/// <para/>
/// Например, стоимость упаковки товара
/// </summary>
public class ExpenseUnit
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }

    public ICondition? Condition { get; set; }
    public ICalculationMethod CalculationMethod { get; set; }

    public bool IsActive { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }

    [JsonIgnore]
    public List<string> RequiredKeys { get; } = [];

    [JsonConstructor]
    public ExpenseUnit(string name, string displayName, ICalculationMethod calculationMethod, DateTimeOffset validFrom, DateTimeOffset? validTo = null, ICondition? condition = null, string? description = null, bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition;
        CalculationMethod = calculationMethod ?? throw new ArgumentNullException(nameof(calculationMethod));
        IsActive = isActive;
        ValidFrom = validFrom;
        ValidTo = validTo;
        RequiredKeys.AddRange(CalculationMethod.RequiredKeys);
        if (Condition is not null)
            RequiredKeys.AddRange(Condition.RequiredKeys);
    }

    public bool TryCalculate(Dictionary<string, object> context, out decimal amount)
    {
        amount = 0;
        if (Condition is not null && !Condition.IsSatisfied(context))
            return false;

        amount = CalculationMethod.Calculate(context);
        return true;
    }
}
