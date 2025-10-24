using System.Text.Json.Serialization;

namespace Quantex.Core.Domain.Calculations;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$calculationType")]
[JsonDerivedType(typeof(FixedAmountCalculation), "fixed")]
[JsonDerivedType(typeof(PercentageCalculation), "percentage")]
[JsonDerivedType(typeof(StepRangeCalculation), "step")]
[JsonDerivedType(typeof(TieredRangeCalculation), "tiered")]
[JsonDerivedType(typeof(OppositeAmountCalculation), "opposite")]
[JsonDerivedType(typeof(SumAmountCalculation), "sum")]
[JsonDerivedType(typeof(MaxAmountCalculation), "max")]
[JsonDerivedType(typeof(MinAmountCalculation), "min")]
[JsonDerivedType(typeof(ClampedCalculation), "clamped")]
[JsonDerivedType(typeof(TernaryCalculation), "ternary")]
[JsonDerivedType(typeof(AdditionAmountCalculation), "addition")]
[JsonDerivedType(typeof(MultiplyAmountCalculation), "multiply")]
[JsonDerivedType(typeof(WithAddedContextValueCalculation), "with-added")]
[JsonDerivedType(typeof(WithMultipliedContextValueCalculation), "with-multiplied")]
[JsonDerivedType(typeof(UniversalStepRangeCalculation), "universal-step")]
[JsonDerivedType(typeof(UniversalTieredRangeCalculation), "universal-tiered")]
public interface ICalculationMethod
{
    public List<string> RequiredKeys { get; }
    public decimal Calculate(Dictionary<string, object> context);
}
