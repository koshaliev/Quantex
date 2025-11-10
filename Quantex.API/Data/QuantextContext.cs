using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Quantex.API.Models;

namespace Quantex.API.Data;

public class QuantextContext : DbContext
{
    public DbSet<ExpenseUnitModel> ExpenseUnits { get; set; }
    public DbSet<ExpenseGroupModel> ExpenseGroups { get; set; }
    public DbSet<ExpenseProfileModel> ExpenseProfiles { get; set; }
    public DbSet<ExpenseSchemeModel> ExpenseSchemes { get; set; }

    public QuantextContext(DbContextOptions<QuantextContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("main");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(QuantextContext))!);
    }
}
