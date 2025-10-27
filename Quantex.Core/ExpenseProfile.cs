using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Quantex.Core.Conditions;

namespace Quantex.Core;

/// <summary>
/// Профиль расходов
/// <para/>
/// Например, список групп расходов
/// </summary>
public class ExpenseProfile
{
    [JsonIgnore]
    private HashSet<string>? _requiredKeys;

    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public ICondition Condition { get; set; }
    public List<ExpenseGroup> ExpenseGroups { get; } = [];

    [JsonIgnore]
    public HashSet<string> RequiredKeys
    {
        get
        {
            if (_requiredKeys is null)
            {
                _requiredKeys = new HashSet<string>();

                for (int i = 0; i < Condition.RequiredKeys.Count; i++)
                {
                    _requiredKeys.Add(Condition.RequiredKeys[i]);
                }

                for (int i = 0; i < ExpenseGroups.Count; i++)
                {
                    for (int j = 0; j < ExpenseGroups[i].RequiredKeys.Count; j++)
                    {
                        _requiredKeys.Add(ExpenseGroups[i].RequiredKeys[j]);
                    }
                }
            }

            return _requiredKeys;

        }
    }

    public ExpenseProfile(string name, string displayName, ICondition condition, List<ExpenseGroup> expenseGroups, string? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));

        ArgumentNullException.ThrowIfNull(expenseGroups);
        if (expenseGroups.Count == 0)
            throw new ArgumentException("At least one expense group must be provided.", nameof(expenseGroups));
        ExpenseGroups = expenseGroups;
    }

    public bool TryCalculate(Dictionary<string, object> context, [NotNullWhen(true)] out Dictionary<string, Dictionary<string, decimal>>? totalExpenses)
    {
        totalExpenses = null;
        if (!Condition.IsSatisfied(context))
            return false;

        totalExpenses = [];
        for (int i = 0; i < ExpenseGroups.Count; i++)
        {
            if (ExpenseGroups[i].TryCalculate(context, out var expenseAmounts))
                totalExpenses[ExpenseGroups[i].Name] = expenseAmounts!;
        }
        return true;
    }
}
