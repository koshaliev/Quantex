namespace Quantex.Core.SourceGeneration;

public readonly record struct CalculationToGenerate
{
    public readonly string Discriminator;
    public readonly string FullName;

    public CalculationToGenerate(string fullName, string discriminator)
    {
        FullName = fullName;
        Discriminator = discriminator;
    }
}
