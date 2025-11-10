using System.Text.Json.Serialization;

namespace Quantex.Core;

/// <summary>
/// Схема расходов
/// <para/>
/// Например, список профилей расходов
/// </summary>
public class ExpenseScheme
{
    public Guid Id { get; set; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public List<ExpenseProfile> Profiles { get; init; }

    [JsonIgnore]
    public Dictionary<string, HashSet<string>> RequiredKeysByProfileName { get; } = [];

    public ExpenseScheme(Guid id, string name, List<ExpenseProfile> profiles, string? description = null)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;

        ArgumentNullException.ThrowIfNull(profiles);
        if (profiles.Count == 0)
            throw new ArgumentException("At least one expense profile must be provided.", nameof(profiles));
        Profiles = profiles;

        for (int i = 0; i < Profiles.Count; i++)
            RequiredKeysByProfileName[Profiles[i].Name] = Profiles[i].RequiredKeys;
    }
}
