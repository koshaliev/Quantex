using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

public sealed class NotCondition : ICondition
{
    public ICondition Condition { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => Condition.RequiredKeys;

    public NotCondition(ICondition condition)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }

    public bool IsSatisfied(Dictionary<string, object> context) => !Condition.IsSatisfied(context);
}
