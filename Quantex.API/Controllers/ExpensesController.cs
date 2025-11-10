using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quantex.API.Data;
using Quantex.API.Models;

namespace Quantex.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ExpensesController(QuantextContext dbContext) : ControllerBase
{
    [HttpPost("new-example")]
    public async Task<IActionResult> GenerateExampleAndReturn()
    {
        var unit = new ExpenseUnit(
            id: Guid.NewGuid(),
            name: "Delivery",
            displayName: "Доставка",
            condition: new GreaterThanOrEqualCondition("price", 1000),
            calculation: new TernaryCalculation(
            condition: new LessThanOrEqualCondition("price", 300),
            ifTrue: new StepRangeCalculation(
                key: "volume",
                ranges: [
                    new StepRangeRule(0.0m, 30m, 400, StepRangeRuleType.Fixed),
                    new StepRangeRule(30m, 190, 792, StepRangeRuleType.Fixed),
                    new StepRangeRule(190m, decimal.MaxValue, 1000, StepRangeRuleType.Fixed),
                    ]),
            ifFalse: new UniversalStepRangeCalculation(
                key: "volume",
                ranges: [
                    new UniversalStepRangeRule(0, 3, new FixedCalculation(200)),
                    new UniversalStepRangeRule(3, 190,
                        new SumCalculation([
                                new FixedCalculation(116),
                                new ContextValueAdditionCalculation("volume", -3, new MultiplicationCalculation("volume", 23))
                            ])
                        ),
                    new UniversalStepRangeRule(190, decimal.MaxValue, new FixedCalculation(5000)),
                    ])),
            validFrom: DateTimeOffset.UtcNow.AddDays(-1));

        var group = new ExpenseGroup(Guid.NewGuid(), "DELIVERY_GROUP", "Группа Доставка", units: [unit]);

        var profile = new ExpenseProfile(
            id: Guid.NewGuid(),
            name: "Default",
            displayName: "Профиль Бытовая техника",
            condition: new EqualsStringCondition("product_type", "Бытовая техника"),
            groups: [group]);

        var scheme = new ExpenseScheme(
            id: Guid.NewGuid(),
            name: "Default",
            profiles: [profile],
            description: null);

        var schemeModel = ExpenseSchemeModel.FromDomain(scheme);

        dbContext.Add(schemeModel);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}
