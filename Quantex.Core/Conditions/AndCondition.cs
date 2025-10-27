using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

/// <summary>
/// Условие, которое объединяет несколько вложенных условий логическим оператором «И» и или истинно, если все вложенные условия истинны.
/// </summary>
public sealed class AndCondition : ICondition
{
    [JsonIgnore]
    private List<string>? _requiredKeys;
    public List<ICondition> Conditions { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys
    {
        get
        {
            if (_requiredKeys is null)
            {
                _requiredKeys = [];
                for (int i = 0; i < Conditions.Count; i++)
                {
                    for (int j = 0; j < Conditions[i].RequiredKeys.Count; j++)
                        _requiredKeys.Add(Conditions[i].RequiredKeys[j]);
                }
            }

            return _requiredKeys;
        }
    }

    public AndCondition(List<ICondition> conditions)
    {
        Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
    }

    public bool IsSatisfied(Dictionary<string, object> context)
    {
        for (int i = 0; i < Conditions.Count; i++)
        {
            if (Conditions[i].IsSatisfied(context))
                return false;
        }

        return true;
    }
}
