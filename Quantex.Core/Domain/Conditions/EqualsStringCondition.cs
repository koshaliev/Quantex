using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Conditions;

public sealed class EqualsStringCondition : ICondition
{
    public string Key { get; }
    public string ExpectedValue { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    [JsonConstructor]
    public EqualsStringCondition(string key, string expectedValue)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        ExpectedValue = expectedValue;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is string sValue && ExpectedValue.Equals(sValue, StringComparison.OrdinalIgnoreCase);
}
