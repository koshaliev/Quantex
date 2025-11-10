using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantex.API.Models;

public sealed class ExpenseProfileModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }
    public required string ConditionJSON { get; set; }
    public List<ExpenseGroupModel> Groups { get; set; } = [];

    public Guid SchemeId { get; set; }
    public ExpenseSchemeModel? Scheme { get; set; }

    public static ExpenseProfileModel FromDomain(ExpenseProfile profile)
    {
        var profileModelId = profile.Id;

        var condition = JsonSerializer.Serialize(profile.Condition, QuantextJsonSerializerOptions.WithoutIndentInstance);

        var groupModels = new List<ExpenseGroupModel>(profile.Groups.Count);
        for (int i = 0; i < profile.Groups.Count; i++)
        {
            var groupModel = ExpenseGroupModel.FromDomain(profile.Groups[i]);
            groupModel.ProfileId = profileModelId;
            groupModels.Add(groupModel);
        }

        var model = new ExpenseProfileModel
        {
            Id = profile.Id,
            Name = profile.Name,
            DisplayName = profile.DisplayName,
            Description = profile.Description,
            ConditionJSON = condition,
            Groups = groupModels
        };
        return model;
    }

    public ExpenseProfile ToDomain()
    {
        var condition = JsonSerializer.Deserialize<ICondition>(ConditionJSON, QuantextJsonSerializerOptions.Default);
        if (condition is null)
            throw new InvalidOperationException($"The JSON in {nameof(ConditionJSON)} must not deserialize to null");

        var domainGroups = new List<ExpenseGroup>(Groups.Count);
        for (int i = 0; i < Groups.Count; i++)
        {
            var domainGroup = Groups[i].ToDomain();
            domainGroups.Add(domainGroup);
        }

        var profile = new ExpenseProfile(
            id: Id,
            name: Name,
            displayName: DisplayName,
            description: Description,
            condition: condition,
            groups: domainGroups);
        return profile;
    }
}

public sealed class ExpenseProfileModelConfiguration : IEntityTypeConfiguration<ExpenseProfileModel>
{
    public void Configure(EntityTypeBuilder<ExpenseProfileModel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ConditionJSON)
            .HasColumnType("json");

        builder.HasMany(x => x.Groups)
            .WithOne(z => z.Profile)
            .HasForeignKey(c => c.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
