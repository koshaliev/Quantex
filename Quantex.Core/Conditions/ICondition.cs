using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$conditionType")]
[JsonDerivedType(typeof(EqualsNumberCondition), "==n")]
[JsonDerivedType(typeof(EqualsStringCondition), "==s")]
[JsonDerivedType(typeof(GreaterThanCondition), ">")]
[JsonDerivedType(typeof(LessThanCondition), "<")]
[JsonDerivedType(typeof(GreaterThanOrEqualCondition), ">=")]
[JsonDerivedType(typeof(LessThanOrEqualCondition), "<=")]
[JsonDerivedType(typeof(AndCondition), "and")]
[JsonDerivedType(typeof(OrCondition), "or")]
[JsonDerivedType(typeof(NotCondition), "not")]
public interface ICondition
{
    public List<string> RequiredKeys { get; }
    bool IsSatisfied(Dictionary<string, object> context);
}
