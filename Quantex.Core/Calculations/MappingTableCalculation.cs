using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который возвращает результат, соответствующий значению из контекста, на основе набора правил сопоставления.
/// </summary>
public sealed class MappingTableCalculation : ICalculationMethod
{
    public string Key { get; init; }

    public List<MappingRule> Rules { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public MappingTableCalculation(string key, List<MappingRule> rules)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        Key = key;
        Rules = rules;

        Rules.Sort((a, b) => a.When.CompareTo(b.When));
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(MappingTableCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        for (int i = 0; i < Rules.Count;i++)
        {
            if (decimalValue == Rules[i].When)
                return Rules[i].Then;
        }
        throw new ArgumentException($"No rule found for value {decimalValue}");
    }
}

/// <summary>
/// Метод, который возвращает значение, соответствующее ближайшему большему правилу из таблицы сопоставления.
/// Если значение меньше минимального - используется первое правило, если больше максимального - последнее.
/// </summary>
public sealed class ForwardMappingTableCalculation : ICalculationMethod
{
    public string Key { get; init; }

    public List<MappingRule> Rules { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => [Key];

    public ForwardMappingTableCalculation(string key, List<MappingRule> rules)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        Key = key;
        Rules = rules;

        Rules.Sort((a, b) => a.When.CompareTo(b.When));
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        if (!context.TryGetValue(Key, out var value) || value is not decimal decimalValue)
            throw new ArgumentException($"Invalid context for {nameof(MappingTableCalculation)}. Key '{Key}' not found or Value is not decimal in context.");

        if (decimalValue < Rules[0].When)
            return Rules[0].Then;

        if (decimalValue > Rules[Rules.Count - 1].When)
            return Rules[Rules.Count - 1].Then;

        for (int i = 0; i < Rules.Count; i++)
        {
            if (decimalValue == Rules[i].When)
                return Rules[i].Then;
        }

        decimal? nearest = null;

        for (int i = 0; i < Rules.Count; i++)
        {
            if (decimalValue < Rules[i].When)
            {
                nearest = Rules[i].Then;
                break;
            }
        }

        return nearest ?? throw new ArgumentException($"No rule found for value {decimalValue}");
    }
}

public sealed class MappingRule
{
    public decimal When { get; init; }
    public decimal Then { get; init; }

    public MappingRule(decimal when, decimal then)
    {
        When = when;
        Then = then;
    }
}
