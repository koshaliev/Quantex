using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

public sealed class OrCondition : ICondition
{
    public List<ICondition> Conditions { get; }

    [JsonIgnore]
    public List<string> RequiredKeys { get; } = [];

    [JsonConstructor]
    public OrCondition(List<ICondition> conditions)
    {
        Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
        for (int i = 0; i < Conditions.Count; i++)
        {
            if (Conditions[i] is null)
                throw new ArgumentException($"Condition at index {i} is null.", nameof(conditions));
            RequiredKeys.AddRange(Conditions[i].RequiredKeys);
        }
    }

    public bool IsSatisfied(Dictionary<string, object> context)
    {
        foreach (var condition in Conditions)
        {
            if (condition.IsSatisfied(context))
                return true;
        }

        return false;
    }
}
