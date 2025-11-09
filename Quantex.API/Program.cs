using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Quantex.API.Data;
using Serilog;

Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, lc) =>
{
    lc.ReadFrom.Configuration(builder.Configuration);
    lc.Enrich.FromLogContext();
});

builder.Services.AddDbContext<QuantextContext>(x =>
{
    var connString = builder.Configuration.GetConnectionString("DefaultConnection");
    ArgumentException.ThrowIfNullOrWhiteSpace(connString);
    x.LogTo(x => Debug.WriteLine(x));
    x.UseNpgsql(connString);
});

builder.Services.AddControllers();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
