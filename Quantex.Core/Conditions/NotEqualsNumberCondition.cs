using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

/// <summary>
/// Условие, которое проверяет, не равно ли числовое значение в контексте заданному числу.
/// </summary>
[Condition("!=n")]
public sealed class NotEqualsNumberCondition : ICondition
{
    public string Key { get; init; }
    public decimal Value { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public NotEqualsNumberCondition(string key, decimal value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is decimal dValue && dValue != Value;
}

