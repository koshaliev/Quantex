using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

/// <summary>
/// Условие, которое объединяет несколько вложенных условий логическим оператором «ИЛИ» и истинно, если хотя бы одно из вложенных условий истинно.
/// </summary>
public sealed class OrCondition : ICondition
{
    public List<ICondition> Conditions { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [];

    public OrCondition(List<ICondition> conditions)
    {
        Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
        for (int i = 0; i < Conditions.Count; i++)
        {
            ArgumentNullException.ThrowIfNull(Conditions[i]);
            for (int j = 0; j < Conditions[i].RequiredKeys.Count; j++)
                RequiredKeys.Add(Conditions[i].RequiredKeys[j]);
        }
    }

    public bool IsSatisfied(Dictionary<string, object> context)
    {
        for (int i = 0; i < Conditions.Count; i++)
        {
            if (Conditions[i].IsSatisfied(context))
                return true;
        }

        return false;
    }
}
