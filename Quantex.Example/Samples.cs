using System.Text.Encodings.Web;
using System.Text.Json;
using Quantex.Core;
using Quantex.Core.Calculations;
using Quantex.Core.Conditions;

namespace Quantex.Example;

internal static class Samples
{
    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        IndentSize = 2,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    internal static void ShowSimple()
    {
        Console.WriteLine("\n\t\t ======== Simple sample ========");
        var deliveryExpense = new ExpenseUnit(
        name: "Delivery",
        displayName: "Доставка",
        condition: new GreaterThanOrEqualCondition("price", 1000),
        calculationMethod: new TernaryCalculation(
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

        var deliveryExpenseGroup = new ExpenseGroup("DELIVERY_GROUP", "Группа Доставка", expenses: [deliveryExpense]);

        var profile = new ExpenseProfile(
            name: "Default",
            displayName: "Профиль Бытовая техника",
            condition: new EqualsStringCondition("product_type", "Бытовая техника"),
            groups: [deliveryExpenseGroup]);

        var json = JsonSerializer.Serialize(profile, _jsonOptions);
        Console.WriteLine(json);
        Console.WriteLine("\n------------------------------\n");
    }


    internal static void ShowAdvanced()
    {
        // -------------------------------
        // SALE_GROUP
        Console.WriteLine("\n\t\t ======== Advanced sample ========");
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
                        new StepRangeRule(0.0m, 30m, 400, StepRangeRuleType.Fixed),
                new StepRangeRule(30m, 190, 792, StepRangeRuleType.Fixed),
                new StepRangeRule(190m, decimal.MaxValue, 1000, StepRangeRuleType.Fixed),
                        ]),
                ifFalse: new UniversalStepRangeCalculation(
                    "volume",
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
                    new StepRangeRule(0, 100, 90, StepRangeRuleType.Fixed),
            new StepRangeRule(100, 200, 190, StepRangeRuleType.Fixed),
            new StepRangeRule(200, decimal.MaxValue, 450, StepRangeRuleType.Fixed),
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
        // Creating profile
        var profile = new ExpenseProfile(
            name: "Default",
            displayName: "Профиль Электроника",
            condition: new EqualsStringCondition("product_category", "Электроника"),
            groups: [saleCommissionGroup, deliveryExpenseGroup, productPurchaseGroup, productPackagingGroup]);

        // serialization
        var jsonProfile = JsonSerializer.Serialize(profile, _jsonOptions);
        Console.WriteLine(jsonProfile);
        Console.WriteLine("\n------------------------------\n");

        // deserialization
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
    }


    /// <summary>
    /// Демонстрация работы системы расчета расходов на примере статьи "Логистика FBO Ozon"
    /// <para/>
    /// <seealso href="https://seller-edu.ozon.ru/commissions-tariffs/legal-information/full-actual-commissions#2-1-1-логистика"/>
    /// <para/>
    /// Логистика заказа на Ozon зависит от "Среднего времени доставки", "Объема товара", "Цены товара".
    /// </summary>
    internal static void ShowOzonFboLogistics()
    {
        var cheapProductDelivery = new StepRangeCalculation(
            key: "volume",
            ranges: [
                new(0m, 0.2m, 17, StepRangeRuleType.Fixed),
                new(0.2m, 0.4m, 19, StepRangeRuleType.Fixed),
                new(0.4m, 0.6m, 21, StepRangeRuleType.Fixed),
                new(0.6m, 0.8m, 22, StepRangeRuleType.Fixed),
                new(0.8m, 1m, 23, StepRangeRuleType.Fixed),
                new(1m, 1.25m, 25, StepRangeRuleType.Fixed),
                new(1.25m, 1.5m, 26, StepRangeRuleType.Fixed),
                new(1.5m, 1.75m, 27, StepRangeRuleType.Fixed),
                new(1.75m, 2m, 29, StepRangeRuleType.Fixed),
                new(2m, 3m, 31, StepRangeRuleType.Fixed),
                new(3m, 4m, 35, StepRangeRuleType.Fixed),
                new(4m, 5m, 38, StepRangeRuleType.Fixed),
                new(5m, 6m, 42, StepRangeRuleType.Fixed),
                new(6m, 7m, 57, StepRangeRuleType.Fixed),
                new(7m, 8m, 61, StepRangeRuleType.Fixed),
                new(8m, 9m, 64, StepRangeRuleType.Fixed),
                new(9m, 10m, 68, StepRangeRuleType.Fixed),
                new(10m, 11m, 78, StepRangeRuleType.Fixed),

                new(11m, 12m, 82, StepRangeRuleType.Fixed),
                new(12m, 13m, 86, StepRangeRuleType.Fixed),
                new(13m, 14m, 91, StepRangeRuleType.Fixed),
                new(14m, 15m, 95, StepRangeRuleType.Fixed),
                new(15m, 17m, 100, StepRangeRuleType.Fixed),
                new(17m, 20m, 109, StepRangeRuleType.Fixed),

                new(20m, 25m, 117, StepRangeRuleType.Fixed),
                new(25m, 30m, 144, StepRangeRuleType.Fixed),
                new(30m, 35m, 144, StepRangeRuleType.Fixed),

                new(35m, 40m, 154, StepRangeRuleType.Fixed),
                new(40m, 45m, 173, StepRangeRuleType.Fixed),
                new(45m, 50m, 186, StepRangeRuleType.Fixed),
                
                new(50m, 60m, 204, StepRangeRuleType.Fixed),
                new(60m, 70m, 227, StepRangeRuleType.Fixed),

                new(70m, 80m, 245, StepRangeRuleType.Fixed),
                new(80m, 90m, 270, StepRangeRuleType.Fixed),

                new(90m, 100m, 280, StepRangeRuleType.Fixed),
                new(100m, 125m, 326, StepRangeRuleType.Fixed),

                new(125m, 150m, 375, StepRangeRuleType.Fixed),
                new(150m, 175m, 429, StepRangeRuleType.Fixed),
                new(175m, 190m, 476, StepRangeRuleType.Fixed),
                new(190m, decimal.MaxValue, 792, StepRangeRuleType.Fixed),
                ]);

        var standartPriceDelivery = new UniversalStepRangeCalculation(
            key: "volume",
            ranges: [
                new(0, 1m, new FixedCalculation(46)),
                new(1, 2m, new FixedCalculation(56)),
                new(2, 3m, new FixedCalculation(66)),
                new(3, 190m, new SumCalculation([
                    new FixedCalculation(66),
                    new ContextValueAdditionCalculation(
                        key: "volume",
                        value: -3,
                        calculation: new MultiplicationCalculation(
                            key: "volume",
                            value: 15m))
                        ])),
                new(190m, decimal.MaxValue, new FixedCalculation(2871))
                ]);

        var deliveryCalculation = new TernaryCalculation(
            condition: new LessThanOrEqualCondition("price", 300),
            ifTrue: cheapProductDelivery,
            ifFalse: standartPriceDelivery);

        var logisticsCoefCalculation = new MappingTableCalculation(
            key: "avg_delivery_time",
            rules: [
                new(29, 1.000m),
                new(30, 1.050m),
                new(31, 1.110m),
                new(32, 1.160m),
                new(33, 1.230m),
                new(34, 1.280m),
                new(35, 1.320m),
                new(36, 1.360m),
                new(37, 1.400m),
                new(38, 1.440m),
                new(39, 1.480m),
                new(40, 1.510m),
                new(41, 1.540m),
                new(42, 1.570m),
                new(43, 1.600m),
                new(44, 1.630m),
                new(45, 1.660m),
                new(46, 1.690m),
                new(47, 1.710m),
                new(48, 1.730m),
                new(49, 1.750m),
                new(50, 1.760m),
                new(51, 1.770m),
                new(52, 1.774m),
                new(53, 1.780m),
                new(54, 1.784m),
                new(55, 1.788m),
                new(56, 1.790m),
                new(57, 1.792m),
                new(58, 1.794m),
                new(59, 1.796m),
                new(60, 1.798m),
                new(61, 1.800m)
                ]);


        var markupOnPriceCalculation = new ForwardMappingTableCalculation(
            key: "avg_delivery_time",
            rules: [
                new(29, 0.0000m),
                new(30, 0.0025m),
                new(31, 0.0055m),
                new(32, 0.0080m),
                new(33, 0.0115m),
                new(34, 0.0140m),
                new(35, 0.0160m),
                new(36, 0.0180m),
                new(37, 0.0200m),
                new(38, 0.0220m),
                new(39, 0.0240m),
                new(40, 0.0255m),
                new(41, 0.0270m),
                new(42, 0.0285m),
                new(43, 0.0300m),
                new(44, 0.0315m),
                new(45, 0.0330m),
                new(46, 0.0345m),
                new(47, 0.0355m),
                new(48, 0.0365m),
                new(49, 0.0375m),
                new(50, 0.0380m),
                new(51, 0.0385m),
                new(52, 0.0387m),
                new(53, 0.0390m),
                new(54, 0.0392m),
                new(55, 0.0394m),
                new(56, 0.0395m),
                new(57, 0.0396m),
                new(58, 0.0397m),
                new(59, 0.0398m),
                new(60, 0.0399m),
                new(61, 0.0400m)
                ]);

        var cached = new CachedContextCalculation([
            new("#delivery", deliveryCalculation),
            new("#logistics", new ProductCalculation([
                logisticsCoefCalculation, 
                deliveryCalculation
                ])),
            new("#markup", new ProductCalculation([
                markupOnPriceCalculation,
                new OnlyContextValueCalculation("price")
                ])),
            new("#result", new SumCalculation([
                new OnlyContextValueCalculation("#markup"),
                new OnlyContextValueCalculation("#logistics")
                ]))
            ]);

        var logisticsExpense = new ExpenseUnit(
            name: "OzonFboLogistics",
            displayName: "Логистика FBO",
            calculationMethod: cached,
            validFrom: DateTimeOffset.MinValue,
            validTo: null,
            condition: null,
            description: "",
            isActive: true);

        var json = JsonSerializer.Serialize(logisticsExpense, _jsonOptions);
        Console.WriteLine(json);
        Console.WriteLine($"Required keys: {string.Join("; ", logisticsExpense.RequiredKeys.ToHashSet())}");
        var ctx = new Dictionary<string, object>
        {
            ["price"] = 301m,
            ["volume"] = 176m,
            ["avg_delivery_time"] = 61m
        };

        logisticsExpense.TryCalculate(ctx, out var result);
        var expectedLogistics = (decimal)ctx["price"] * 0.04m + (66 + ((decimal)ctx["volume"] - 3)*15) * 1.8m;
        Console.WriteLine($"Expected: {expectedLogistics}. Actual: {result}" );
    }
}
