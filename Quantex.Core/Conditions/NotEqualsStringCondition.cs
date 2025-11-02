using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

/// <summary>
/// Условие, которое проверяет, не совпадает ли строковое значение в контексте с заданной строкой.
/// </summary>
[Condition("!=s")]
public sealed class NotEqualsStringCondition : ICondition
{
    public string Key { get; }
    public string Value { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public NotEqualsStringCondition(string key, string value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is string sValue && !Value.Equals(sValue, StringComparison.InvariantCultureIgnoreCase);
}
