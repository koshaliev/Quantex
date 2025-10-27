using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;

/// <summary>
/// Метод, который возвращает противоположное по знаку значение, полученное в результате вложенного (подчиненного) вычисления.
/// </summary>
public sealed class OppositeAmountCalculation : ICalculationMethod
{
    public ICalculationMethod Calculation { get; init; }

    [JsonIgnore]
    public List<string> RequiredKeys => Calculation.RequiredKeys;

    [JsonConstructor]
    public OppositeAmountCalculation(ICalculationMethod calculation)
    {
        Calculation = calculation ?? throw new ArgumentNullException(nameof(calculation));
    }

    public decimal Calculate(Dictionary<string, object> context) => -Calculation.Calculate(context);
}
