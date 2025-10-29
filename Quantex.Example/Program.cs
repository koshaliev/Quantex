using System.Text.Encodings.Web;
using System.Text.Json;
using Quantex.Core;
using Quantex.Core.Calculations;
using Quantex.Core.Conditions;

// -------------------------------
// SALE_GROUP
var saleCommissionExpense = new ExpenseUnit(
    name: "SaleComission",
    displayName: "Комиссия за продажу",
    calculationMethod: new PercentageCalculation("price", 13.5m),
    validFrom: DateTimeOffset.MinValue,
    validTo: null,
    condition: new EqualsStringCondition("product_category", "Электроника"),
    description: "Комиссия за продажу в категории \"Электроника\"",
    isActive: true);

var acquiringExpense = new ExpenseUnit(
    name: "Acquiring",
    displayName: "Эквайринг",
    calculationMethod: new PercentageCalculation("price", 1.5m),
    validFrom: DateTimeOffset.MinValue,
    validTo: null,
    condition: null,
    description: null,
    isActive: true);

var saleCommissionGroup = new ExpenseGroup(
    name: "SALE_GROUP",
    displayName: "Группа Продажа",
    expenses: [saleCommissionExpense, acquiringExpense],
    description: null,
    condition: null);


// -------------------------------
// DELIVERY_GROUP
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
    validFrom: DateTimeOffset.MinValue);

var lastMileExpense = new ExpenseUnit(
    name: "LastMile",
    displayName: "Последняя мила",
    calculationMethod: new ClampedCalculation(new PercentageCalculation("price", 5.5m), 30, 500),
    validFrom: DateTimeOffset.MinValue,
    validTo: null,
    condition: new NotEqualsStringCondition("destination_city", "Moscow"),
    description: "Доставка товара до конечного покупателя в городе назначения",
    isActive: true);

var deliveryExpenseGroup = new ExpenseGroup(
    name: "DELIVERY_GROUP",
    displayName: "Группа Доставка",
    expenses: [deliveryExpense, lastMileExpense]);


// -------------------------------
// PACKAGING_GROUP
var packingByEmployeeExpense = new ExpenseUnit(
    name: "PackingByEmployee",
    displayName: "Упаковка товара сотрудником",
    calculationMethod: new ClampedCalculation(
        calculation: new PercentageCalculation("purchase_price", 1.1m), 
        minAmount: null, 
        maxAmount: 50),
    validFrom: DateTimeOffset.MinValue,
    validTo: null,
    condition: new EqualsStringCondition("sale_scheme", "FBS"),
    description: null,
    isActive: true);

var productPackageExpense = new ExpenseUnit(
    name: "ProductPackage",
    displayName: "Упаковочная тара для товара",
    calculationMethod: new StepRangeCalculation(
        key: "volume", 
        ranges: [
            new StepRangeRule(0, 100, 90, StepRangeRuleType.FixedAmount),
            new StepRangeRule(100, 200, 190, StepRangeRuleType.FixedAmount),
            new StepRangeRule(200, decimal.MaxValue, 450, StepRangeRuleType.FixedAmount),
            ]),
    validFrom: DateTimeOffset.MinValue,
    validTo: null,
    condition: new EqualsStringCondition("product_category", "Электроника"),
    description: null,
    isActive: true);

var productPackagingGroup = new ExpenseGroup(
    name: "PACKAGING_GROUP",
    displayName: "Группа Упаковка",
    expenses: [packingByEmployeeExpense, productPackageExpense]);


// -------------------------------
// PRODUCT_PURCHASE_GROUP
var productPurchaseExpense = new ExpenseUnit(
    name: "ProductPurchase",
    displayName: "Закупочная цена товара",
    calculationMethod: new OnlyContextValueCalculation("purchase_price"),
    validFrom: DateTimeOffset.MinValue,
    validTo: null,
    condition: null,
    description: null,
    isActive: true);

var productPurchaseGroup = new ExpenseGroup(
    name: "PRODUCT_PURCHASE_GROUP",
    displayName: "Группа Закупка товара",
    expenses: [productPurchaseExpense]);


// -------------------------------
var profile = new ExpenseProfile(
    name: "Default",
    displayName: "Профиль Электроника",
    condition: new EqualsStringCondition("product_category", "Электроника"),
    groups: [saleCommissionGroup, deliveryExpenseGroup, productPurchaseGroup, productPackagingGroup]);

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
Console.WriteLine($"Required keys: {string.Join(", ", deserializedProfile.RequiredKeys)}");

Console.WriteLine("\n------------------------------\n");
var ctx = new Dictionary<string, object> 
{
    { "sale_scheme", "FBS" },
    { "destination_city", "New York" },
    { "price", 35000m }, 
    { "volume", 162m }, 
    { "product_category", "Электроника" }, 
    { "purchase_price", 12100m } 
};
Console.WriteLine("Total expenses:");
var totalCost = 0m;
if (deserializedProfile.TryCalculate(ctx, out var totalExpenses))
{
    foreach (var groupKV in totalExpenses)
    {
        var groupCost = 0m;
        Console.WriteLine($"  Group: {groupKV.Key}");
        foreach (var expense in groupKV.Value)
        {
            Console.WriteLine($"    Name: {expense.Key}\tValue: {expense.Value}");
            groupCost += expense.Value;
        }
        Console.WriteLine($"  Group total cost: {groupCost}\n  -----------");
        totalCost += groupCost;
    }
    Console.WriteLine($"Total cost: {totalCost}");
}
