using System.Text.Json.Serialization;

namespace Quantex.Core;

/// <summary>
/// Единица расхода. 
/// <para/>
/// Например, стоимость упаковки товара
/// </summary>
public sealed class ExpenseUnit
{
    [JsonIgnore]
    private List<string>? _requiredKeys;

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }

    public ICondition? Condition { get; set; }
    public ICalculationMethod Calculation { get; set; }

    public bool IsActive { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }

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

                for (int i = 0; i < Calculation.RequiredKeys.Count; i++)
                {
                    _requiredKeys.Add(Calculation.RequiredKeys[i]);
                }
            }
            return _requiredKeys;

        }
    }
    public ExpenseUnit(Guid id, string name, string displayName, ICalculationMethod calculation, DateTimeOffset validFrom, DateTimeOffset? validTo = null, ICondition? condition = null, string? description = null, bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Condition = condition;
        Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));
        IsActive = isActive;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public bool TryCalculate(Dictionary<string, object> context, out decimal amount)
    {
        amount = 0;
        if (Condition is not null && !Condition.IsSatisfied(context))
            return false;

        amount = Calculation.Calculate(context);
        return true;
    }
}
