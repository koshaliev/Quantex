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

    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public ICondition? Condition { get; set; }
    public List<ExpenseUnit> Expenses { get; set; }

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

                for (int i = 0; i < Expenses.Count; i++)
                {
                    for (int j = 0; j < Expenses[i].RequiredKeys.Count; j++)
                    {
                        _requiredKeys.Add(Expenses[i].RequiredKeys[j]);
                    }
                }
            }
            return _requiredKeys;
        }
    }

    public ExpenseGroup(string name, string displayName, List<ExpenseUnit> expenses, string? description = null, ICondition? condition = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition;

        ArgumentNullException.ThrowIfNull(expenses);
        if (expenses.Count == 0)
            throw new ArgumentException("At least one expense unit must be provided.", nameof(expenses));
        Expenses = expenses;
    }

    public bool TryCalculate(Dictionary<string, object> context, [NotNullWhen(true)] out Dictionary<string, decimal>? expenseAmounts)
    {
        expenseAmounts = null;
        if (Condition is not null && !Condition.IsSatisfied(context))
            return false;

        expenseAmounts = [];
        for (int i = 0; i < Expenses.Count; i++)
        {
            if (Expenses[i].TryCalculate(context, out var amount))
                expenseAmounts[Expenses[i].Name] = amount;
        }
        return true;
    }
}
