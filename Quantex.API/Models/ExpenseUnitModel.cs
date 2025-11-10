using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantex.API.Models;

public sealed class ExpenseUnitModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }
    public string? ConditionJSON { get; set; }
    public required string CalculationJSON { get; set; }
    public required bool IsActive { get; set; }
    public required DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }

    public Guid GroupId { get; set; }
    public ExpenseGroupModel? Group { get; set; }

    public static ExpenseUnitModel FromDomain(ExpenseUnit unit)
    {
        var calculation = JsonSerializer.Serialize(unit.Calculation, QuantextJsonSerializerOptions.WithoutIndentInstance);

        string? condition = null;
        if (unit.Condition is not null)
            condition = JsonSerializer.Serialize(unit.Condition, QuantextJsonSerializerOptions.WithoutIndentInstance);

        var model = new ExpenseUnitModel
        {
            Id = unit.Id,
            Name = unit.Name,
            DisplayName = unit.DisplayName,
            Description = unit.Description,
            ConditionJSON = condition,
            CalculationJSON = calculation,
            ValidFrom = unit.ValidFrom,
            ValidTo = unit.ValidTo,
            IsActive = unit.IsActive,
        };
        return model;
    }

    public ExpenseUnit ToDomain()
    {
        var calculationMethod = JsonSerializer.Deserialize<ICalculationMethod>(CalculationJSON, QuantextJsonSerializerOptions.Default);
        if (calculationMethod is null)
            throw new InvalidOperationException($"The JSON in {nameof(CalculationJSON)} must not deserialize to null");

        ICondition? condition = null;
        if (ConditionJSON is not null)
        {
            condition = JsonSerializer.Deserialize<ICondition>(ConditionJSON, QuantextJsonSerializerOptions.Default);
            if (condition is null)
                throw new InvalidOperationException($"The JSON in {nameof(ConditionJSON)} must not deserialize to null");
        }

        var unit = new ExpenseUnit(
            id: Id,
            name: Name,
            displayName: DisplayName,
            description: Description,
            condition: condition,
            calculation: calculationMethod,
            validFrom: ValidFrom,
            validTo: ValidTo,
            isActive: IsActive);

        return unit;
    }
}

public sealed class ExpenseUnitModelConfiguration : IEntityTypeConfiguration<ExpenseUnitModel>
{
    public void Configure(EntityTypeBuilder<ExpenseUnitModel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ConditionJSON)
            .HasColumnType("json");

        builder.Property(x => x.CalculationJSON)
            .HasColumnType("json");
    }
}