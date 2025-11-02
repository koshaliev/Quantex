using System.Text.Json.Serialization;

namespace Quantex.Core.Calculations;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$calculationType")]
public partial interface ICalculationMethod
{
    public List<string> RequiredKeys { get; }
    public decimal Calculate(Dictionary<string, object> context);
}
