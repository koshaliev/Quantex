using System.Text.Encodings.Web;
using System.Text.Json;
using Quantex.Core;
using Quantex.Core.Calculations;
using Quantex.Core.Conditions;

Console.WriteLine("Hello, Quantex!");

var deliveryExpense = new ExpenseUnit(
    name: "Delivery",
    displayName: "Доставка",
    condition: new GreaterThanOrEqualCondition("price", 1000),
    calculationMethod: new TernaryCalculation(
        new LessThanOrEqualCondition("price", 300),
        ifTrue: new StepRangeCalculation(
            "volume",
            ranges: [
                new StepRangeRule(0.0m, 30m, 400, StepRangeRuleType.FixedAmount),
                new StepRangeRule(30m, 190, 792, StepRangeRuleType.FixedAmount),
                new StepRangeRule(190m, decimal.MaxValue, 1000, StepRangeRuleType.FixedAmount),
                ]),
        ifFalse: new UniversalStepRangeCalculation(
            "volume",
            ranges: [
                new UniversalStepRangeRule(0, 3, new FixedAmountCalculation(200)),
                new UniversalStepRangeRule(3, 190,
                    new SumAmountCalculation([
                            new FixedAmountCalculation(116),
                            new ContextValueAdditionCalculation("volume", -3, new MultiplicationAmountCalculation("volume", 23))
                        ])
                    ),
                new UniversalStepRangeRule(190, decimal.MaxValue, new FixedAmountCalculation(5000)),
                ])),
    validFrom: DateTimeOffset.UtcNow.AddDays(-1));

var deliveryExpenseGroup = new ExpenseGroup("DELIVERY_GROUP", "Группа Доставка", expenses: [deliveryExpense]);

var profile = new ExpenseProfile(
    name: "Default",
    displayName: "Профиль Бытовая техника",
    condition: new EqualsStringCondition("product_type", "Бытовая техника"),
    groups: [deliveryExpenseGroup]);

var jsonOptions = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    IndentSize = 2,
    WriteIndented = true,
};

var jsonProfile = JsonSerializer.Serialize(profile, jsonOptions);
Console.WriteLine(jsonProfile);
Console.WriteLine("\n------------------------------\n");

var deserializedProfile = JsonSerializer.Deserialize<ExpenseProfile>(jsonProfile)!;

Console.WriteLine($"Name: {deserializedProfile.Name}");
Console.WriteLine($"Display name: {deserializedProfile.DisplayName}");
Console.WriteLine($"Required keys: {string.Join(';', deserializedProfile.RequiredKeys)}");

foreach (var group in deserializedProfile.Groups)
{
    Console.WriteLine($"  Group name: {group.Name}");
    foreach (var expense in group.Expenses)
    {
        Console.WriteLine($"    Expense name: {expense.Name}");
    }
}

Console.WriteLine("\n------------------------------\n");
var ctx = new Dictionary<string, object> { { "price", 1300m }, { "volume", 162m }, { "product_type", "Бытовая техника" } };
Console.WriteLine("Expenses");
if (deserializedProfile.TryCalculate(ctx, out var totalExpenses))
{
    foreach (var groupKV in totalExpenses)
    {
        Console.WriteLine($"  Group: {groupKV.Key}");
        foreach (var expense in groupKV.Value)
        {
            Console.WriteLine($"    Name: {expense.Key}\tValue: {expense.Value}");
        }
    }
}
