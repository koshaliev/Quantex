using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Quantex.API.Data;
using Quantex.API.Models;
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
    x.EnableDetailedErrors(true);
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

//var unit = new ExpenseUnit(
//    id: Guid.NewGuid(),
//    name: "Delivery",
//    displayName: "Доставка",
//    condition: new GreaterThanOrEqualCondition("price", 1000),
//    calculation: new TernaryCalculation(
//    condition: new LessThanOrEqualCondition("price", 300),
//    ifTrue: new StepRangeCalculation(
//        key: "volume",
//        ranges: [
//            new StepRangeRule(0.0m, 30m, 400, StepRangeRuleType.Fixed),
//            new StepRangeRule(30m, 190, 792, StepRangeRuleType.Fixed),
//            new StepRangeRule(190m, decimal.MaxValue, 1000, StepRangeRuleType.Fixed),
//            ]),
//    ifFalse: new UniversalStepRangeCalculation(
//            key: "volume",
//            ranges: [
//                new UniversalStepRangeRule(0, 3, new FixedCalculation(200)),
//                new UniversalStepRangeRule(3, 190,
//                    new SumCalculation([
//                            new FixedCalculation(116),
//                            new ContextValueAdditionCalculation("volume", -3, new MultiplicationCalculation("volume", 23))
//                        ])
//                    ),
//                new UniversalStepRangeRule(190, decimal.MaxValue, new FixedCalculation(5000)),
//                ])),
//    validFrom: DateTimeOffset.UtcNow.AddDays(-1));

//var model = ExpenseUnitModel.FromDomain(unit);
//var scope = app.Services.CreateScope();
//var dbContext = scope.ServiceProvider.GetRequiredService<QuantextContext>();
//dbContext.Add(model);
//await dbContext.SaveChangesAsync();

//var fromDbModel = await dbContext.ExpenseUnits
//    .AsNoTracking()
//    .FirstAsync(x => x.Id == unit.Id);
//var fromDbUnit = fromDbModel!.ToDomain();

//var json = JsonSerializer.Serialize(fromDbUnit, QuantextJsonSerializerOptions.Default);
//Log.Logger.Information(json);
