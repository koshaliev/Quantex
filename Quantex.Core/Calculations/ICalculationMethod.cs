using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$calculationType")]
[JsonDerivedType(typeof(FixedCalculation), "fixed")]
[JsonDerivedType(typeof(PercentageCalculation), "percentage")]
[JsonDerivedType(typeof(StepRangeCalculation), "step")]
[JsonDerivedType(typeof(TieredRangeCalculation), "tiered")]
[JsonDerivedType(typeof(OppositeCalculation), "opposite")]
[JsonDerivedType(typeof(SumCalculation), "sum")]
[JsonDerivedType(typeof(MaxCalculation), "max")]
[JsonDerivedType(typeof(MinCalculation), "min")]
[JsonDerivedType(typeof(ClampedCalculation), "clamped")]
[JsonDerivedType(typeof(TernaryCalculation), "?")]
[JsonDerivedType(typeof(AdditionCalculation), "+")]
[JsonDerivedType(typeof(MultiplicationCalculation), "*")]
[JsonDerivedType(typeof(ContextValueAdditionCalculation), "ctx:+")]
[JsonDerivedType(typeof(ContextValueMultiplicationCalculation), "ctx:*")]
[JsonDerivedType(typeof(UniversalStepRangeCalculation), "universal-step")]
[JsonDerivedType(typeof(OnlyContextValueCalculation), "only-ctx-value")]
[JsonDerivedType(typeof(ProductCalculation), "product")]
[JsonDerivedType(typeof(MappingTableCalculation), "mapping-table")]
[JsonDerivedType(typeof(ForwardMappingTableCalculation), "forward-mapping-table")]
[JsonDerivedType(typeof(ScopedContextCalculation), "scoped-ctx")]
public interface ICalculationMethod
{
    public List<string> RequiredKeys { get; }
    public decimal Calculate(Dictionary<string, object> context);
}
