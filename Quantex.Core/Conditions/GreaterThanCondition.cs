using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

public sealed class GreaterThanCondition : ICondition
{
    public string Key { get; }
    public decimal Value { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    [JsonConstructor]
    public GreaterThanCondition(string key, decimal value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is decimal dValue && dValue > Value;
}
