namespace Quantex.Core.SourceGeneration;

public static class SourceGenerationHelper
{
    public const string CalculationMethodAttributeText = """
        namespace Quantex.Core.Attributes;

        [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
        [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public sealed class CalculationMethodAttribute : System.Attribute
        {
            public string Name { get; }

            public CalculationMethodAttribute(string name)
            {
                Name = name;
            }
        }
        """;

    public const string ConditionMethodAttributeText = """
        namespace Quantex.Core.Attributes;

        [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
        [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public sealed class ConditionAttribute : System.Attribute
        {
            public string Name { get; }

            public ConditionAttribute(string name)
            {
                Name = name;
            }
        }
        """;
}
