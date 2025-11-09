using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Quantex.API.Data;

public class QuantextContext : DbContext
{
    public QuantextContext(DbContextOptions<QuantextContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(QuantextContext))!);
    }
}
