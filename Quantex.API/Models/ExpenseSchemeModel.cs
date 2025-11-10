using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantex.API.Models;

public sealed class ExpenseSchemeModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<ExpenseProfileModel> Profiles { get; set; } = [];

    public static ExpenseSchemeModel FromDomain(ExpenseScheme scheme)
    {
        var schemeModelId = scheme.Id;
        var profiles = new List<ExpenseProfileModel>(scheme.Profiles.Count);
        for (int i = 0; i < scheme.Profiles.Count; i++)
        {
            var profile = ExpenseProfileModel.FromDomain(scheme.Profiles[i]);
            profile.SchemeId = schemeModelId;
            profiles.Add(profile);
        }

        var model = new ExpenseSchemeModel
        {
            Id = schemeModelId,
            Name = scheme.Name,
            Description = scheme.Description,
            Profiles = profiles
        };
        return model;
    }

    public ExpenseScheme ToDomain()
    {
        var domainProfiles = new List<ExpenseProfile>(Profiles.Count);
        for (int i = 0; i < Profiles.Count; i++)
        {
            var domainProfile = Profiles[i].ToDomain();
            domainProfiles.Add(domainProfile);
        }

        var scheme = new ExpenseScheme(
            id: Id,
            name: Name,
            description: Description,
            profiles: domainProfiles);
        return scheme;
    }
}

public sealed class ExpenseSchemeModelConfiguration : IEntityTypeConfiguration<ExpenseSchemeModel>
{
    public void Configure(EntityTypeBuilder<ExpenseSchemeModel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasMany(x => x.Profiles)
            .WithOne(z => z.Scheme)
            .HasForeignKey(c => c.SchemeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}