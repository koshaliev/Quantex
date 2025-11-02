using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

/// <summary>
/// Условие, которое возвращает true, если числовое значение в контексте больше или равно заданному порогу
/// </summary>
[Condition(">=")]
public sealed class GreaterThanOrEqualCondition : ICondition
{
    public string Key { get; }
    public decimal Value { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public GreaterThanOrEqualCondition(string key, decimal value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is decimal dValue && dValue >= Value;
}
