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

Например, группа **Упаковка товара** может включать следующие расходы:
- стоимость коробки - 20 у.е.
- стоимость упаковки работником - 1% от цены товара.

---

### 3. [ExpenseProfile](./Quantex.Core/ExpenseProfile.cs)
Профиль объединяет несколько `ExpenseGroup`.
Позволяет описать полный набор расходов при выполнении заданного условия (например, конкретный тип товара или категория).

Например, профиль **Одежда** может включать следующие группы:
- `Комиссия площадки`
- `Доставка`
- `Упаковка товара`

---

### 4. [ExpenseScheme](./Quantex.Core/ExpenseScheme.cs)
Схема объединяет несколько `ExpenseProfile`.

Например, 
- схема **Реализация через площадку А** из 5 шт `ExpenseProfile`
- схема **Реализация через площадку Б** из 10 шт `ExpenseProfile`
- схема **Реализация через площадку В** из 35 шт `ExpenseProfile`

---

#### Методы вычисления (реализации интерфейса `ICalculationMethod`)
- [FixedCalculation](./Quantex.Core/Calculations/FixedCalculation.cs) - Возвращает заданную фиксированную сумму.
- [PercentageCalculation](./Quantex.Core/Calculations/PercentageCalculation.cs) - Возвращает сумму, составляющую указанный процент от значения, содержащегося в контексте.
- [StepRangeCalculation](./Quantex.Core/Calculations/StepRangeCalculation.cs) - Выбирает значение из шагового диапазона на основе входных данных.
- [TieredRangeCalculation](./Quantex.Core/Calculations/TieredRangeCalculation.cs) - Вычисляет результат путём суммирования значений по соответствующим диапазонам (уровням).
- [OppositeCalculation](./Quantex.Core/Calculations/OppositeCalculation.cs) - Возвращает противоположное по знаку значение, полученное в результате вложенного (подчиненного) вычисления.
- [SumCalculation](./Quantex.Core/Calculations/SumCalculation.cs) - Суммирует результаты, полученные от нескольких вложенных (подчиненных) вычислений.
- [MaxCalculation](./Quantex.Core/Calculations/MaxCalculation.cs) - Возвращает максимальное значение среди результатов вложенных (подчиненных) вычислений.
- [MinCalculation](./Quantex.Core/Calculations/MinCalculation.cs) - Возвращает минимальное значение среди результатов вложенных (подчиненных) вычислений.
- [ClampedCalculation](./Quantex.Core/Calculations/ClampedCalculation.cs) - Ограничивает результат вложенного вычисления заданными минимальным и максимальным значениями.
- [TernaryCalculation](./Quantex.Core/Calculations/TernaryCalculation.cs) - Выбирает и выполняет одно из двух вычислений в зависимости от результата проверки заданного условия.
- [AdditionCalculation](./Quantex.Core/Calculations/AdditionCalculation.cs) - Прибавляет фиксированное значение к значению из контекста и возвращает итоговый результат.
- [SubtractionCalculation](./Quantex.Core/Calculations/SubtractionCalculation.cs) - Отнимает фиксированное значение из значения в контексте и возвращает итоговый результат.
- [MultiplicationCalculation](./Quantex.Core/Calculations/MultiplicationCalculation.cs) - Умножает значение из контекста на фиксированное число и возвращает итоговый результат.
- [DivisionCalculation](./Quantex.Core/Calculations/DivisionCalculation.cs) - Делит значение из контекста на фиксированное число и возвращает итоговый результат.
- [ContextValueAdditionCalculation](./Quantex.Core/Calculations/ContextValueAdditionCalculation.cs) - Временно добавляет фиксированное число к значению из контекста перед выполнением вычисления, а затем восстанавливает исходное значение контекста.
- [ContextValueMultiplicationCalculation](./Quantex.Core/Calculations/ContextValueMultiplicationCalculation.cs) - Временно умножает значение из контекста на фиксированное число перед выполнением вычисления, а затем восстанавливает исходное значение контекста.
- [UniversalStepRangeCalculation](./Quantex.Core/Calculations/UniversalStepRangeCalculation.cs) - Выбирает вложенное вычисление, соответствующее шаговому диапазону на основе входных данных, и выполняет его, возвращая полученный результат.
- [OnlyContextValueCalculation](./Quantex.Core/Calculations/OnlyContextValueCalculation.cs) - Возвращает значение из контекста.

- [ProductCalculation](./Quantex.Core/Calculations/ProductCalculation.cs) - Перемножает результаты, полученные от нескольких вложенных (подчиненных) вычислений.
- [MappingTableCalculation](./Quantex.Core/Calculations/MappingTableCalculation.cs) - Возвращает результат, соответствующий значению из контекста, на основе набора правил сопоставления.
- [ForwardMappingTableCalculation](./Quantex.Core/Calculations/ForwardMappingTableCalculation.cs) - Метод, который возвращает значение, соответствующее ближайшему большему правилу из таблицы сопоставления. 
Если значение меньше минимального - используется первое правило, если больше максимального - последнее.
- [CachedContextCalculation](./Quantex.Core/Calculations/CachedContextCalculation.cs) - Вычисляет результат вложенных методов и сохраняет их в отдельном контексте под заданным ключом.
При повторных вызовах возвращает ранее сохранённое значение из отдельного контекста, не выполняя повторное вычисление.
Всегда возвращает результат последнего вложенного метода вычисления.

---

#### Условия (реализации интерфейса `ICondition`)
- [EqualsNumberCondition](./Quantex.Core/Conditions/EqualsNumberCondition.cs) - Проверяет, равно ли числовое значение в контексте заданному числу.
- [NotEqualsNumberCondition](./Quantex.Core/Conditions/NotEqualsNumberCondition.cs) - Проверяет, не равно ли числовое значение в контексте заданному числу.
- [EqualsStringCondition](./Quantex.Core/Conditions/EqualsStringCondition.cs) - Проверяет, совпадает ли строковое значение в контексте с заданной строкой.
- [NotEqualsStringCondition](./Quantex.Core/Conditions/NotEqualsStringCondition.cs) - Проверяет, не совпадает ли строковое значение в контексте с заданной строкой.
- [GreaterThanCondition](./Quantex.Core/Conditions/GreaterThanCondition.cs) - Возвращает true, если числовое значение в контексте больше указанного порога.
- [GreaterThanOrEqualCondition](./Quantex.Core/Conditions/GreaterThanOrEqualCondition.cs) - Возвращает true, если числовое значение в контексте больше или равно заданному порогу.
- [LessThanCondition](./Quantex.Core/Conditions/LessThanCondition.cs) - Возвращает true, если числовое значение в контексте меньше указанного порога.
- [LessThanOrEqualCondition](./Quantex.Core/Conditions/LessThanOrEqualCondition.cs) - Возвращает true, если числовое значение меньше или равно заданному порогу.
- [AndCondition](./Quantex.Core/Conditions/AndCondition.cs) - Объединяет несколько условий логическим «И» и истинно, если все условия истинны.
- [OrCondition](./Quantex.Core/Conditions/OrCondition.cs) - Объединяет несколько условий логическим «ИЛИ» и истинно, если хотя бы одно условие истинно.
- [NotCondition](./Quantex.Core/Conditions/NotCondition.cs) - Инвертирует результат вложенного (подчиненного) условия.

---

### Пример

Несмотря на большое количество различных методов вычисления и условий, все реализации поддерживают сериализацию и десериализацию.  
Это позволяет вручную описывать схемы и правила расчёта в JSON.

Например, следующее определение:
```csharp
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
    "Value": "Бытовая техника"
  },
  "Groups": [
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
                  "Type": "Fixed"
                },
                {
                  "From": 30,
                  "To": 190,
                  "Value": 792,
                  "Type": "Fixed"
                },
                {
                  "From": 190,
                  "To": 79228162514264337593543950335,
                  "Value": 1000,
                  "Type": "Fixed"
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
                    "Value": 200
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
                        "Value": 116
                      },
                      {
                        "$calculationType": "ctx:+",
                        "Key": "volume",
                        "Value": -3,
                        "Calculation": {
                          "$calculationType": "*",
                          "Key": "volume",
                          "Value": 23
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
                    "Value": 5000
                  }
                }
              ]
            }
          },
          "IsActive": true,
          "ValidFrom": "2025-10-28T18:21:28.2697322+00:00",
          "ValidTo": null
        }
      ]
    }
  ]
}
```

