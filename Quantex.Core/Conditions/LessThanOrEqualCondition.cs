using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

/// <summary>
/// Условие, которое возвращает true, если числовое значение в контексте меньше или равно заданному порогу.
/// </summary>
public sealed class LessThanOrEqualCondition : ICondition
{
    public string Key { get; }
    public decimal Value { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public LessThanOrEqualCondition(string key, decimal value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is decimal dValue && dValue <= Value;
}
