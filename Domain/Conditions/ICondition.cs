using System.Text.Json.Serialization;

namespace Quantex.Domain.Conditions;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$conditionType")]
[JsonDerivedType(typeof(EqualsNumberCondition), "eqn")]
[JsonDerivedType(typeof(EqualsStringCondition), "eqs")]
[JsonDerivedType(typeof(GreaterThanCondition), "gt")]
[JsonDerivedType(typeof(LessThanCondition), "lt")]
[JsonDerivedType(typeof(GreaterThanOrEqualCondition), "gte")]
[JsonDerivedType(typeof(LessThanOrEqualCondition), "lte")]
[JsonDerivedType(typeof(AndCondition), "and")]
[JsonDerivedType(typeof(OrCondition), "or")]
[JsonDerivedType(typeof(NotCondition), "not")]
public interface ICondition
{
    public List<string> RequiredKeys { get; }
    bool IsSatisfied(Dictionary<string, object> context);
}
