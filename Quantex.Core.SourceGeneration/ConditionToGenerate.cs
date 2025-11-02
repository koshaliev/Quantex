namespace Quantex.Core.SourceGeneration;

public readonly record struct ConditionToGenerate
{
    public readonly string Discriminator;
    public readonly string FullName;

    public ConditionToGenerate(string fullName, string discriminator)
    {
        FullName = fullName;
        Discriminator = discriminator;
    }
}
