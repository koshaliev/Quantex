using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$conditionType")]
public partial interface ICondition
{
    public List<string> RequiredKeys { get; }
    bool IsSatisfied(Dictionary<string, object> context);
}
