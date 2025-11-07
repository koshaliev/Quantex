using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который вычисляет результат вложенных методов и сохраняет их в отдельном контексте под заданным ключом.
/// Всегда возвращает результат последнего вложенного метода вычисления.
/// </summary>
[CalculationMethod("scoped-ctx")]
public sealed class ScopedContextCalculation : ICalculationMethod
{
    [JsonIgnore]
    private List<string>? _requiredKeys;

    [JsonIgnore]
    private readonly Dictionary<string, object> _scopedContext = [];

    public List<InnerCachedCalculation> CachedCalculations { get; } = [];

    [JsonIgnore]
    public List<string> RequiredKeys
    {
        get
        {
            if (_requiredKeys is null)
            {
                _requiredKeys = [];
                for (int i = 0; i < CachedCalculations.Count; i++)
                {
                    for (int j = 0; j < CachedCalculations[i].Calculation.RequiredKeys.Count; j++)
                        _requiredKeys.Add(CachedCalculations[i].Calculation.RequiredKeys[j]);
                }
            }
            return _requiredKeys;
        }
    }

    public ScopedContextCalculation(List<InnerCachedCalculation> cachedCalculations)
    {
        CachedCalculations = cachedCalculations ?? throw new ArgumentNullException(nameof(cachedCalculations));
        if (CachedCalculations.Count == 0)
            throw new ArgumentException("At least one inner cachced calculation method must be provided.", nameof(CachedCalculations));
    }

    public decimal Calculate(Dictionary<string, object> context)
    {
        string resultKey = null!;
        for (int i = 0; i < CachedCalculations.Count; i++)
        {
            for (int j = 0; j < CachedCalculations[i].RequiredKeys.Count; j++)
            {
                var requiredKey = CachedCalculations[i].RequiredKeys[j];
                if (_scopedContext.ContainsKey(requiredKey))
                    continue;

                if (!context.TryGetValue(requiredKey, out var value) || value is not decimal decimalValue)
                    throw new ArgumentException($"Invalid context for {nameof(MappingTableCalculation)}. Key '{requiredKey}' not found or Value is not decimal in context.");

                _scopedContext[requiredKey] = context[requiredKey];
            }

            resultKey = CachedCalculations[i].ResultKey;
            _scopedContext[resultKey] = CachedCalculations[i].Calculate(_scopedContext);
        }

        return (decimal)_scopedContext[resultKey];
    }

    public sealed class InnerCachedCalculation
    {
        public string ResultKey { get; init; }
        public ICalculationMethod Calculation { get; init; }

        [JsonIgnore]
        public List<string> RequiredKeys => Calculation.RequiredKeys;

        public InnerCachedCalculation(string resultKey, ICalculationMethod calculation)
        {
            ArgumentException.ThrowIfNullOrEmpty(resultKey);
            ResultKey = resultKey;
            Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));
        }

        public decimal Calculate(Dictionary<string, object> context)
        {
            return Calculation.Calculate(context);
        }
    }
}

