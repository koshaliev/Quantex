using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Quantex.Core.SourceGeneration;

[Generator]
public class ConditionGenerator : IIncrementalGenerator
{
    private const string markerAttribute = "global::Quantex.Core.Attributes.ConditionAttribute";
    private const string markerInterface = "global::Quantex.Core.Conditions.ICondition";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddEmbeddedAttributeDefinition();
            ctx.AddSource("ConditionAttribute.g.cs", SourceText.From(SourceGenerationHelper.ConditionMethodAttributeText, Encoding.UTF8));
        });

        var conditions = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsSyntaxTargetForGeneration(node),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(x => x is not null)
            .Collect();

        context.RegisterSourceOutput(conditions, Generate);
    }

    static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0 && cds.BaseList != null;

    static CalculationToGenerate? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var cds = (ClassDeclarationSyntax)context.Node;

        foreach (AttributeListSyntax attributeListSyntax in cds.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (fullName == markerAttribute)
                {
                    return GetConditionToGenerate(context.SemanticModel, cds);
                }
            }
        }

        return null;
    }

    static CalculationToGenerate? GetConditionToGenerate(SemanticModel semanticModel, SyntaxNode cds)
    {
        if (semanticModel.GetDeclaredSymbol(cds) is not INamedTypeSymbol classSymbol)
        {
            return null;
        }

        if (!classSymbol.AllInterfaces.Any(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == markerInterface))
            return null;

        string? descriminator = null;
        foreach (var attribute in classSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == markerAttribute)
            {
                if (attribute.ConstructorArguments.Length > 0)
                {
                    descriminator = attribute.ConstructorArguments[0].Value?.ToString();
                    break;
                }
            }
        }

        string className = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return new CalculationToGenerate(className, descriminator!);
    }

    static void Generate(SourceProductionContext context, ImmutableArray<CalculationToGenerate?> conditionsToGenerate)
    {

        if (conditionsToGenerate.IsDefaultOrEmpty)
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("namespace Quantex.Core.Conditions;");
        sb.AppendLine();

        foreach (var condition in conditionsToGenerate)
        {
            if (condition is null)
                continue;

            var disc = condition.Value.Discriminator.ToLowerInvariant();
            var name = condition.Value.FullName;
            sb.AppendLine($"[System.Text.Json.Serialization.JsonDerivedType(typeof({name}), \"{disc}\")]");
        }

        sb.AppendLine("public partial interface ICondition { }");
        var result = sb.ToString();
        context.AddSource("ICondition.g.cs", SourceText.From(result, Encoding.UTF8));
    }
}
