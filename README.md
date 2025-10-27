# Quantex

**Quantex** - ядро для построения логики вычислений.  
Позволяет комбинировать методы и условия, создавая сложные вычислительные модели.

## Архитектура

Структура системы состоит из четырех уровней:

### 1. [ExpenseUnit](./Quantex.Core/ExpenseUnit.cs)
Единица расхода - минимальный элемент расчета.

Состоит из двух компонентов:
- [ICalculationMethod](./Quantex.Core/Calculations/ICalculationMethod.cs) - определяет метод вычисления
- [ICondition](./Quantex.Core/Conditions/ICondition.cs) - задает условие применения данной единицы расхода

Например, 20% от цены товара.

---

### 2. [ExpenseGroup](./Quantex.Core/ExpenseGroup.cs)
Группа расходов - объединяет (группирует) несколько `ExpenseUnit`, относящиеся к одной логической категории. 

Например, группа **Упаковка товара** может включать:
- стоимость коробки - 20 у.е.
- стоимость упаковки работником - 1% от цены товара.

---

### 3. [ExpenseProfile](./Quantex.Core/ExpenseProfile.cs)
Профиль объединяет несколько `ExpenseGroup`.
Позволяет описать полный набор расходов при выполнении заданного условия (например, конкретный тип товара или категория).

Например, профиль **Одежда** может включать:
- `Комиссия площадки`
- `Доставка`
- `Упаковка товара`

---

### 4. [ExpenseScheme](./Quantex.Core/ExpenseScheme.cs)
Схема объединяет несколько `ExpenseProfile`.

Например, 
- схема **Реализация через площадку А** 
- схема **Реализация через площадку Б**
- схема **Реализация через площадку В**

---

#### Методы вычисления (реализации интерфейса `ICalculationMethod`)
- [FixedAmountCalculation](./Quantex.Core/Calculations/FixedAmountCalculation.cs) - возвращает фиксированную заданную сумму.
- [PercentageCalculation](./Quantex.Core/Calculations/PercentageCalculation.cs) - вычисляет процент от значения, указанного в контексте.
- [StepRangeCalculation](./Quantex.Core/Calculations/StepRangeCalculation.cs) - выбирает значение по шаговому диапазону, соответствующему входным данным.
- [TieredRangeCalculation](./Quantex.Core/Calculations/TieredRangeCalculation.cs) - вычисляет результат на основе суммирования значений по диапазонам
- [SumAmountCalculation](./Quantex.Core/Calculations/SumAmountCalculation.cs) - суммирует результаты нескольких вложенных вычислений.
- [MaxAmountCalculation](./Quantex.Core/Calculations/MaxAmountCalculation.cs) - возвращает максимальное значение среди результатов подвычислений.
- [MinAmountCalculation](./Quantex.Core/Calculations/MinAmountCalculation.cs) - возвращает минимальное значение среди результатов подвычислений.
- [ClampedCalculation](./Quantex.Core/Calculations/ClampedCalculation.cs) - ограничивает результат вычисления заданными минимальным и максимальным значениями.
- [TernaryCalculation](./Quantex.Core/Calculations/TernaryCalculation.cs) - выбирает одно из двух вычислений в зависимости от условия.
- [AdditionAmountCalculation](./Quantex.Core/Calculations/AdditionAmountCalculation.cs) - прибавляет фиксированное значение к значению из контекста и возвращает результат, не изменяя контекст.
- [MultiplicationAmountCalculation](./Quantex.Core/Calculations/MultiplicationAmountCalculation.cs) - умножает значение из контекста на фиксированное число и возвращает результат, не изменяя контекст.
- [ContextValueAdditionCalculation](./Quantex.Core/Calculations/ContextValueAdditionCalculation.cs) - временно добавляет к значению из контекста фиксированное число перед вычислением и после завершения возвращает исходное значение.
- [ContextValueMultiplicationCalculation](./Quantex.Core/Calculations/ContextValueMultiplicationCalculation.cs) - временно умножает значение из контекста на фиксированный коэффициент перед вычислением и затем восстанавливает исходное значение.

---

#### Условия (реализации интерфейса `ICondition`)
- [EqualsNumberCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - проверяет, равно ли числовое значение в контексте заданному числу.
- [EqualsStringCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - проверяет, совпадает ли строковое значение в контексте с заданной строкой.
- [GreaterThanCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - возвращает true, если числовое значение в контексте больше указанного порога.
- [LessThanCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - возвращает true, если числовое значение в контексте меньше указанного порога.
- [LessThanOrEqualCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - возвращает true, если числовое значение меньше или равно заданному порогу.
- [AndCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - объединяет несколько условий логическим «И» и выполняется, если все условия истинны.
- [OrCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - объединяет несколько условий логическим «ИЛИ» и выполняется, если хотя бы одно условие истинно.
- [NotCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - инвертирует результат вложенного условия.

---

Несмотря на большое количество различных методов вычисления и условий, все реализации поддерживают сериализацию и десериализацию.  
Это позволяет вручную описывать схемы и правила расчёта в JSON.


### Пример

Данная схема:
```csharp
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
    expenseGroups: [deliveryExpenseGroup]);
```

сериализуется в:

```json
{
  "Name": "Default",
  "DisplayName": "Профиль Бытовая техника",
  "Description": null,
  "Condition": {
    "$conditionType": "==s",
    "Key": "product_type",
    "Expected": "Бытовая техника"
  },
  "ExpenseGroups": [
    {
      "Name": "DELIVERY_GROUP",
      "DisplayName": "Группа Доставка",
      "Description": null,
      "Condition": null,
      "Expenses": [
        {
          "Name": "Delivery",
          "DisplayName": "Доставка",
          "Description": null,
          "Condition": {
            "$conditionType": ">=",
            "Key": "price",
            "Value": 1000
          },
          "CalculationMethod": {
            "$calculationType": "?",
            "Condition": {
              "$conditionType": "<=",
              "Key": "price",
              "Value": 300
            },
            "IfTrue": {
              "$calculationType": "step",
              "Key": "volume",
              "Ranges": [
                {
                  "From": 0.0,
                  "To": 30,
                  "Value": 400,
                  "Type": "FixedAmount"
                },
                {
                  "From": 30,
                  "To": 190,
                  "Value": 792,
                  "Type": "FixedAmount"
                },
                {
                  "From": 190,
                  "To": 79228162514264337593543950335,
                  "Value": 1000,
                  "Type": "FixedAmount"
                }
              ]
            },
            "IfFalse": {
              "$calculationType": "universal-step",
              "Key": "volume",
              "Ranges": [
                {
                  "From": 0,
                  "To": 3,
                  "Calculation": {
                    "$calculationType": "fixed",
                    "Amount": 200
                  }
                },
                {
                  "From": 3,
                  "To": 190,
                  "Calculation": {
                    "$calculationType": "sum",
                    "Calculations": [
                      {
                        "$calculationType": "fixed",
                        "Amount": 116
                      },
                      {
                        "$calculationType": "ctx:+",
                        "Key": "volume",
                        "Amount": -3,
                        "Calculation": {
                          "$calculationType": "*",
                          "Key": "volume",
                          "Multiplier": 23
                        }
                      }
                    ]
                  }
                },
                {
                  "From": 190,
                  "To": 79228162514264337593543950335,
                  "Calculation": {
                    "$calculationType": "fixed",
                    "Amount": 5000
                  }
                }
              ]
            }
          },
          "IsActive": true,
          "ValidFrom": "2025-10-26T00:00:01.0000000+00:00",
          "ValidTo": null
        }
      ]
    }
  ]
}
```

