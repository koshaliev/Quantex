using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Conditions;

public sealed class EqualsNumberCondition : ICondition
{
    public string Key { get; init; }
    public decimal ExpectedValue { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    [JsonConstructor]
    public EqualsNumberCondition(string key, decimal expectedValue)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        ExpectedValue = expectedValue;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is decimal dValue && dValue == ExpectedValue;
}
