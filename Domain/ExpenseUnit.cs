using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Quantex.Domain.Calculations;
using Quantex.Domain.Conditions;

namespace Quantex.Domain;

/// <summary>
/// Единица расхода. 
/// <para/>
/// Например, стоимость упаковки товара
/// </summary>
public class ExpenseUnit
{
    public int Id { get; private set; }
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

/// <summary>
/// Группа расходов
/// <para/>
/// Например, список расходов для определенной категории: [Упаковка, Доставка, Комиссия]
/// </summary>
public class ExpenseGroup
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public ICondition Condition { get; set; }
    public IReadOnlyList<ExpenseUnit> Expenses { get; set; }

    [JsonIgnore]
    public List<string> RequiredKeys { get; } = [];

    [JsonConstructor]
    public ExpenseGroup(string name, string displayName, string? description, ICondition condition, List<ExpenseUnit> expenses)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        Expenses = expenses ?? throw new ArgumentNullException(nameof(expenses));

        RequiredKeys.AddRange(Condition.RequiredKeys);
        for (int i = 0; i < Expenses.Count; i++)
        {
            RequiredKeys.AddRange(Expenses[i].RequiredKeys);
        }
    }

    public bool TryCalculate(Dictionary<string, object> context, [NotNullWhen(true)] out Dictionary<string, decimal>? expenseAmounts)
    {
        expenseAmounts = null;
        if (!Condition.IsSatisfied(context))
            return false;

        expenseAmounts = [];
        for (int i = 0; i < Expenses.Count; i++)
        {
            if (Expenses[i].TryCalculate(context, out var amount))
            {
                expenseAmounts[Expenses[i].Name] = amount;
            }
        }
        return true;
    }
}

public class ExpenseProfile
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public ICondition Condition { get; set; }
    public List<ExpenseGroup> ExpenseGroups { get; }

    [JsonIgnore]
    public HashSet<string> RequiredKeys { get; } = [];

    [JsonConstructor]
    public ExpenseProfile(string name, string displayName, string? description, ICondition condition, List<ExpenseGroup> expenseGroups)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        ExpenseGroups = expenseGroups ?? throw new ArgumentNullException(nameof(expenseGroups));

        for (int i = 0; i < Condition.RequiredKeys.Count; i++)
        {
            RequiredKeys.Add(Condition.RequiredKeys[i]);
        }

        for (int i = 0; i < ExpenseGroups.Count; i++)
        {
            for (int j = 0; j < ExpenseGroups[i].RequiredKeys.Count; j++)
            {
                RequiredKeys.Add(ExpenseGroups[i].RequiredKeys[j]);
            }
        }
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
            {
                totalExpenses[ExpenseGroups[i].Name] = expenseAmounts!;
            }
        }
        return true;
    }
}

/// <summary>
/// Схема расходов (список разных групп расходов)
/// </summary>
public class ExpenseScheme
{
    public int Id { get; private set; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public IReadOnlyList<ExpenseProfile> Profiles { get; init; }

    [JsonIgnore]
    public Dictionary<string, HashSet<string>> RequredKeysByProfileName { get; } = [];

    [JsonConstructor]
    public ExpenseScheme(string name, string? description, List<ExpenseProfile> profiles)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Profiles = profiles ?? throw new ArgumentNullException(nameof(profiles));

        for (int i = 0; i < Profiles.Count; i++)
        {
            RequredKeysByProfileName[Profiles[i].Name] = Profiles[i].RequiredKeys;
        }
    }
}
