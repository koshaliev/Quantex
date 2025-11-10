using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantex.API.Models;

public sealed class ExpenseGroupModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }
    public string? ConditionJSON { get; set; }
    public List<ExpenseUnitModel> Units { get; set; } = [];

    public Guid ProfileId { get; set; }
    public ExpenseProfileModel? Profile { get; set; }

    public static ExpenseGroupModel FromDomain(ExpenseGroup group)
    {
        var groupModelId = group.Id;

        string? condition = null;
        if (group.Condition is not null)
            condition = JsonSerializer.Serialize(group.Condition, QuantextJsonSerializerOptions.WithoutIndentInstance);

        var unitModels = new List<ExpenseUnitModel>(group.Units.Count);
        for (var i = 0; i < group.Units.Count; i++)
        {
            var unitModel = ExpenseUnitModel.FromDomain(group.Units[i]);
            unitModel.GroupId = groupModelId;
            unitModels.Add(unitModel);
        }

        var model = new ExpenseGroupModel
        {
            Id = groupModelId,
            Name = group.Name,
            DisplayName = group.DisplayName,
            Description = group.Description,
            ConditionJSON = condition,
            Units = unitModels
        };
        return model;
    }

    public ExpenseGroup ToDomain()
    {
        ICondition? condition = null;
        if (ConditionJSON is not null)
        {
            condition = JsonSerializer.Deserialize<ICondition>(ConditionJSON, QuantextJsonSerializerOptions.Default);
            if (condition is null)
                throw new InvalidOperationException($"The JSON in {nameof(ConditionJSON)} must not deserialize to null");
        }

        var domainUnits = new List<ExpenseUnit>(Units.Count);
        for (int i = 0; i < Units.Count; i++)
        {
            var domainUnit = Units[i].ToDomain();
            domainUnits.Add(domainUnit);
        }

        var group = new ExpenseGroup(
            id: Id,
            name: Name,
            displayName: DisplayName,
            description: Description,
            units: domainUnits,
            condition: condition);
        return group;
    }
}

public sealed class ExpenseGroupModelConfiguration : IEntityTypeConfiguration<ExpenseGroupModel>
{
    public void Configure(EntityTypeBuilder<ExpenseGroupModel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ConditionJSON)
            .HasColumnType("json");

        builder.HasMany(x => x.Units)
            .WithOne(z => z.Group)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}