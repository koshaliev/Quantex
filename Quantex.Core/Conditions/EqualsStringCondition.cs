using System.Text.Json.Serialization;

namespace Quantex.Core.Conditions;

public sealed class EqualsStringCondition : ICondition
{
    public string Key { get; }
    public string Expected { get; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public EqualsStringCondition(string key, string expected)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Expected = expected;
    }

    public bool IsSatisfied(Dictionary<string, object> context) => context.TryGetValue(Key, out var value) && value is string sValue && Expected.Equals(sValue, StringComparison.InvariantCultureIgnoreCase);
}
