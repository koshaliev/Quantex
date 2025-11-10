using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Quantex.Core;

/// <summary>
/// Группа расходов
/// <para/>
/// Например, список расходов для определенной категории: [Упаковка, Доставка, Комиссия]
/// </summary>
public class ExpenseGroup
{
    [JsonIgnore]
    private List<string>? _requiredKeys;

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public ICondition? Condition { get; set; }
    public List<ExpenseUnit> Units { get; set; }

    [JsonIgnore]
    public List<string> RequiredKeys
    {
        get
        {
            if (_requiredKeys is null)
            {
                _requiredKeys = [];
                if (Condition is not null)
                {
                    for (int i = 0; i < Condition.RequiredKeys.Count; i++)
                    {
                        _requiredKeys.Add(Condition.RequiredKeys[i]);
                    }
                }

                for (int i = 0; i < Units.Count; i++)
                {
                    for (int j = 0; j < Units[i].RequiredKeys.Count; j++)
                    {
                        _requiredKeys.Add(Units[i].RequiredKeys[j]);
                    }
                }
            }
            return _requiredKeys;
        }
    }

    public ExpenseGroup(Guid id, string name, string displayName, List<ExpenseUnit> units, string? description = null, ICondition? condition = null)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition;

        ArgumentNullException.ThrowIfNull(units);
        if (units.Count == 0)
            throw new ArgumentException("At least one expense unit must be provided.", nameof(units));
        Units = units;
    }

    public bool TryCalculate(Dictionary<string, object> context, [NotNullWhen(true)] out Dictionary<string, decimal>? expenseAmounts)
    {
        expenseAmounts = null;
        if (Condition is not null && !Condition.IsSatisfied(context))
            return false;

        expenseAmounts = [];
        for (int i = 0; i < Units.Count; i++)
        {
            if (Units[i].TryCalculate(context, out var amount))
                expenseAmounts[Units[i].Name] = amount;
        }
        return true;
    }
}
