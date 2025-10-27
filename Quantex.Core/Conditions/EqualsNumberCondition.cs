using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

/// <summary>
/// Условие, которое проверяет, равно ли числовое значение в контексте заданному числу.
/// </summary>
public sealed class EqualsNumberCondition : ICondition
{
    public string Key { get; init; }
    public decimal Expected { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public EqualsNumberCondition(string key, decimal expected)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Expected = expected;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is decimal dValue && dValue == Expected;
}
