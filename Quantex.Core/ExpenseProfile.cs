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
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public ICondition Condition { get; set; }
    public List<ExpenseGroup> Groups { get; } = [];

    [JsonIgnore]
    public HashSet<string> RequiredKeys => [];

    public ExpenseProfile(string name, string displayName, ICondition condition, List<ExpenseGroup> groups, string? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));

        ArgumentNullException.ThrowIfNull(groups);
        if (groups.Count == 0)
            throw new ArgumentException("At least one expense group must be provided.", nameof(groups));
        Groups = groups;

        for (int i = 0; i < Condition.RequiredKeys.Count; i++)
            RequiredKeys.Add(Condition.RequiredKeys[i]);

        for (int i = 0; i < Groups.Count; i++)
        {
            for (int j = 0; j < Groups[i].RequiredKeys.Count; j++)
                RequiredKeys.Add(Groups[i].RequiredKeys[j]);
        }
    }

    public bool TryCalculate(Dictionary<string, object> context, [NotNullWhen(true)] out Dictionary<string, Dictionary<string, decimal>>? totalExpenses)
    {
        totalExpenses = null;
        if (!Condition.IsSatisfied(context))
            return false;

        totalExpenses = [];
        for (int i = 0; i < Groups.Count; i++)
        {
            if (Groups[i].TryCalculate(context, out var expenseAmounts))
                totalExpenses[Groups[i].Name] = expenseAmounts!;
        }
        return true;
    }
}
