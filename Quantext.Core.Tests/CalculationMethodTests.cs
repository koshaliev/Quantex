using System.Reflection;
using Newtonsoft.Json.Linq;
using Quantex.Core.Calculations;

namespace Quantext.Core.Tests;

public class CalculationMethodTests
{
    // Naming: ClassName_Method(Ctor)_Result

    // ----
    [Fact]
    [Trait("Class", nameof(FixedCalculation))]
    public void FixedAmountCalculation_Constructor_CreateInstance()
    {
        var amount = 100m;

        var calc = new FixedCalculation(amount);

        Assert.NotNull(calc);
        Assert.Equal(amount, calc.Value);
        Assert.Empty(calc.RequiredKeys);
    }

    [Fact]
    [Trait("Class", nameof(FixedCalculation))]
    public void FixedAmountCalculation_Calculate_ReturnsAmount()
    {
        var amount = 100m;
        var calc = new FixedCalculation(amount);
        var ctx = new Dictionary<string, object>();

        var actual = calc.Calculate(ctx);

        Assert.Equal(amount, actual);
    }

    // ----
    [Fact]
    [Trait("Class", nameof(PercentageCalculation))]
    public void PercentageCalculation_Constructor_CreateInstance()
    {
        var key = "key";
        var percentage = 100m;

        var calc = new PercentageCalculation(key, percentage);

        Assert.NotNull(calc);
        Assert.NotNull(calc.Key);
        Assert.Equal(key, calc.Key);
        Assert.Equal(percentage, calc.Value);
        Assert.Contains(key, calc.RequiredKeys);
    }

    [Fact]
    [Trait("Class", nameof(PercentageCalculation))]
    public void PercentageCalculation_Calculate_ReturnsValidAmount()
    {
        decimal amount = 1000m;
        var key = "key";
        decimal percentage = 12.5m;
        var expectedAmount = amount / 100 * percentage;
        var calc = new PercentageCalculation(key, percentage);
        var ctx = new Dictionary<string, object> { { key, amount } };

        var actualAmount = calc.Calculate(ctx);

        Assert.Equal(expectedAmount, actualAmount);
    }

    [Fact]
    [Trait("Class", nameof(PercentageCalculation))]
    public void PercentageCalculation_Calculate_ThrowsExceptionIfKeyIsNull()
    {
        string? key = null;
        var percentage = 12.5m;

        var action = () => { new PercentageCalculation(key!, percentage); };

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    [Trait("Class", nameof(PercentageCalculation))]
    public void PercentageCalculation_Calculate_ThrowsArgumentExceptionIfContextNotContainsRequiredKey()
    {
        string key = "key";
        decimal percentage = 12.5m;
        var ctx = new Dictionary<string, object> { };
        var calc = new PercentageCalculation(key, percentage);

        var action = () => { calc.Calculate(ctx); };

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [Trait("Class", nameof(PercentageCalculation))]
    [InlineData(null)]
    [InlineData("string")]
    [InlineData(1.000001f)]
    [InlineData(2)]
    [InlineData(3d)]
    [InlineData(false)]
    [InlineData(typeof(object))]
    public void PercentageCalculation_Calculate_ThrowsArgumentExceptionIfContextValueIsNotDecimal(object? value)
    {
        string key = "key";
        decimal percentage = 12.5m;
        var ctx = new Dictionary<string, object> { { key, value! } };
        var calc = new PercentageCalculation(key, percentage);

        var action = () => { calc.Calculate(ctx); };

        Assert.Throws<ArgumentException>(action);
    }

    // ----
    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_Constructor_CreateInstance()
    {
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Percentage;

        var range = new StepRangeRule(from, to, value, type);

        Assert.NotNull(range);
        Assert.Equal(from, range.From);
        Assert.Equal(to, range.To);
        Assert.Equal(value, range.Value);
        Assert.Equal(type, range.Type);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_Constructor_ThrowsArgumentExceptionIfEnumValueNotDefined()
    {
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = (StepRangeRuleType)1_000;

        var action = () => { new StepRangeRule(from, to, value, type); };

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_IsInRange_ReturnsTrueIfAmountInRange()
    {
        var amount = 3m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Fixed;
        var range = new StepRangeRule(from, to, value, type);

        var result = range.IsInRange(amount);

        Assert.True(result);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_IsInRange_ReturnsFalseIfAmountInBoundary()
    {
        var amount = 5m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Fixed;
        var range = new StepRangeRule(from, to, value, type);

        var result = range.IsInRange(amount);

        Assert.True(result);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_IsInRange_ReturnsFalseIfAmountNotInRange()
    {
        var amount = 1_000m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Fixed;
        var range = new StepRangeRule(from, to, value, type);

        var result = range.IsInRange(amount);

        Assert.False(result);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_TryGetCost_ReturnsTrueIfAmountInRange()
    {
        var amount = 3m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Fixed;
        var range = new StepRangeRule(from, to, value, type);

        Assert.True(range.TryGetCost(amount, out var actual));
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_TryGetCost_FixedAmountType_ReturnsValidValue()
    {
        var amount = 3m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Fixed;
        var range = new StepRangeRule(from, to, value, type);
        var expectedValue = 99m;

        range.TryGetCost(amount, out var actual);

        Assert.Equal(expectedValue, actual);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_TryGetCost_PercentageType_ReturnsValidValue()
    {
        var amount = 3m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Percentage;
        var range = new StepRangeRule(from, to, value, type);
        var expectedValue = amount / 100m * value;

        range.TryGetCost(amount, out var actual);

        Assert.Equal(expectedValue, actual);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_TryGetCost_ReturnsFalseIfAmountNotInRange()
    {
        var amount = 10m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var type = StepRangeRuleType.Fixed;
        var range = new StepRangeRule(from, to, value, type);

        Assert.False(range.TryGetCost(amount, out _));
    }

    [Fact]
    [Trait("Class", nameof(StepRangeRule))]
    public void StepRangeRule_TryGetCost_ThrowsNotSupportExceptionIfEnumNotDefined()
    {
        var amount = 2m;
        var from = 0m;
        var to = 5m;
        var value = 99m;
        var validType = StepRangeRuleType.Fixed;
        var invalidType = (StepRangeRuleType)1_000;

        var range = new StepRangeRule(from, to, value, validType);
        var prop = typeof(StepRangeRule).GetProperty(nameof(StepRangeRule.Type), BindingFlags.Public | BindingFlags.Instance);
        var setter = prop!.SetMethod!;
        setter.Invoke(range, new object[] { invalidType });

        var action = () => { range.TryGetCost(amount, out _); };

        Assert.Throws<NotSupportedException>(action);
    }

    // ----
    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Constructor_CreateInstance()
    {
        var key = "key";
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(10, 15, 30, StepRangeRuleType.Fixed),
            new StepRangeRule(5, 10, 20, StepRangeRuleType.Fixed),
            new StepRangeRule(15, 20, 40, StepRangeRuleType.Fixed)];

        var calc = new StepRangeCalculation(key, ranges);

        Assert.NotNull(calc);
        Assert.Equal(key, calc.Key);
        Assert.Contains(key, calc.RequiredKeys);
        Assert.NotNull(calc.Ranges);
        Assert.Equal(ranges.Count, calc.Ranges.Count);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Constructor_ThrowsArgumentNullExceptionIfKeyIsNull()
    {
        string? key = null;
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(5, 10, 20, StepRangeRuleType.Fixed)];

        var action = () => { new StepRangeCalculation(key!, ranges); };

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Constructor_ThrowsArgumentNullExceptionIfRangesIsNull()
    {
        string key = "key";
        List<StepRangeRule>? ranges = null;

        var action = () => { new StepRangeCalculation(key, ranges!); };

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Constructor_ThrowsArgumentExceptionIfRangesIsEmpty()
    {
        string key = "key";
        List<StepRangeRule> ranges = [];

        var action = () => { new StepRangeCalculation(key, ranges); };

        Assert.Throws<ArgumentException>(action, x =>
        {
            if (x.Message.Contains("cannot be empty"))
                return null;
            return $"thrown out invalid exception. Message: {x.Message}";
        });
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Constructor_ThrowsArgumentExceptionIfRangesOverlapping()
    {
        string key = "key";
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(6, 10, 20, StepRangeRuleType.Fixed),
            new StepRangeRule(4, 7, 20, StepRangeRuleType.Fixed),
            new StepRangeRule(11, 20, 30, StepRangeRuleType.Fixed)];

        var action = () => { new StepRangeCalculation(key, ranges); };

        Assert.Throws<ArgumentException>(action, x =>
        {
            if (x.Message.Contains("Ranges must be continuous and non-overlapping."))
                return null;
            return $"thrown out invalid exception. Message: {x.Message}";
        });
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Constructor_ThrowsArgumentExceptionIfRangeHasInvalidBorder()
    {
        string key = "key";
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(10, 6, 20, StepRangeRuleType.Fixed)];

        var action = () => { new StepRangeCalculation(key, ranges); };

        Assert.Throws<ArgumentException>(action, x =>
        {
            if (x.Message.Contains("must be less than To "))
                return null;
            return $"thrown out invalid exception. Message: {x.Message}";
        });
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Calculate_ReturnsValidAmount()
    {
        var amount = 6m;
        string key = "key";
        var ctx = new Dictionary<string, object> { { key, amount } };
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(5, 10, 20, StepRangeRuleType.Fixed)];
        var calc = new StepRangeCalculation(key, ranges);
        var expected = 20m;

        var actual = calc.Calculate(ctx);

        Assert.Equal(expected, actual);
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Calculate_ThrowsArgumentExceptionIfContextValueNotFound()
    {
        var key = "key";
        var ctx = new Dictionary<string, object>();
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(5, 10, 20, StepRangeRuleType.Fixed)];
        var calc = new StepRangeCalculation(key, ranges);

        var action = () => { calc.Calculate(ctx); };

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [Trait("Class", nameof(StepRangeCalculation))]
    [InlineData(null)]
    [InlineData("string")]
    [InlineData(1.000001f)]
    [InlineData(2)]
    [InlineData(3d)]
    [InlineData(false)]
    [InlineData(typeof(object))]
    public void StepRangeCalculation_Calculate_ThrowsArgumentExceptionIfContextValueIsNotDecimal(object? value)
    {
        string key = "key";
        var ctx = new Dictionary<string, object> { { key, value! } };
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(5, 10, 20, StepRangeRuleType.Fixed)];
        var calc = new StepRangeCalculation(key, ranges);

        var action = () => { calc.Calculate(ctx); };

        Assert.Throws<ArgumentException>(action, x =>
        {
            if (x.Message.Contains("not found or Value is not decimal in context."))
                return null;
            return $"thrown out invalid exception. Message: {x.Message}";
        });
    }

    [Fact]
    [Trait("Class", nameof(StepRangeCalculation))]
    public void StepRangeCalculation_Calculate_ThrowsArgumentExceptionIfContextValueNotInRange()
    {
        var leftBorder = -0.1m;
        var rightBorder = 10.1m;
        string key = "key";
        List<StepRangeRule> ranges = [
            new StepRangeRule(0, 5, 10, StepRangeRuleType.Fixed),
            new StepRangeRule(5, 10, 20, StepRangeRuleType.Fixed)];
        var calc = new StepRangeCalculation(key, ranges);

        var leftBorderAction = () => { calc.Calculate(new Dictionary<string, object> { { key, leftBorder } }); };
        var rightBorderAction = () => { calc.Calculate(new Dictionary<string, object> { { key, rightBorder } }); };

        Assert.Throws<ArgumentException>(leftBorderAction, x =>
        {
            if (x.Message.Contains("No range found for value"))
                return null;
            return $"thrown out invalid exception. Message: {x.Message}";
        });
        Assert.Throws<ArgumentException>(rightBorderAction, x =>
        {
            if (x.Message.Contains("No range found for value"))
                return null;
            return $"thrown out invalid exception. Message: {x.Message}";
        });
    }
}
