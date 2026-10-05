using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Strategies;

public class FixedRateRebateStrategyTests
{
    private readonly FixedRateRebateStrategy _strategy = new();

    private static Rebate ValidRebate() => new() { Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };

    private static Product ValidProduct() => new()
    {
        Price = 20m,
        SupportedIncentives = SupportedIncentiveType.FixedRateRebate
    };

    private static CalculateRebateRequest ValidRequest() => new() { Volume = 10m };

    [Fact]
    public void Type_IsFixedRateRebate()
    {
        Assert.Equal(IncentiveType.FixedRateRebate, _strategy.Type);
    }

    [Fact]
    public void IsValid_AllInputsValid_ReturnsTrue()
    {
        Assert.True(_strategy.IsValid(ValidRebate(), ValidProduct(), ValidRequest()));
    }

    [Fact]
    public void IsValid_NullRebate_ReturnsFalse()
    {
        Assert.False(_strategy.IsValid(null, ValidProduct(), ValidRequest()));
    }

    [Fact]
    public void IsValid_NullProduct_ReturnsFalse()
    {
        Assert.False(_strategy.IsValid(ValidRebate(), null, ValidRequest()));
    }

    [Fact]
    public void IsValid_ProductDoesNotSupportIncentive_ReturnsFalse()
    {
        var product = ValidProduct();
        product.SupportedIncentives = SupportedIncentiveType.AmountPerUom;

        Assert.False(_strategy.IsValid(ValidRebate(), product, ValidRequest()));
    }

    [Fact]
    public void IsValid_ZeroPercentage_ReturnsFalse()
    {
        var rebate = ValidRebate();
        rebate.Percentage = 0m;

        Assert.False(_strategy.IsValid(rebate, ValidProduct(), ValidRequest()));
    }

    [Fact]
    public void IsValid_ZeroPrice_ReturnsFalse()
    {
        var product = ValidProduct();
        product.Price = 0m;

        Assert.False(_strategy.IsValid(ValidRebate(), product, ValidRequest()));
    }

    [Fact]
    public void IsValid_ZeroVolume_ReturnsFalse()
    {
        var request = ValidRequest();
        request.Volume = 0m;

        Assert.False(_strategy.IsValid(ValidRebate(), ValidProduct(), request));
    }

    [Theory]
    [InlineData(20, 0.1, 10, 20)]
    [InlineData(19.99, 0.5, 2, 19.99)]
    [InlineData(100, 0.25, 4, 100)]
    public void Calculate_ReturnsPriceTimesPercentageTimesVolume(double price, double percentage, double volume, double expected)
    {
        var rebate = ValidRebate();
        rebate.Percentage = (decimal)percentage;
        var product = ValidProduct();
        product.Price = (decimal)price;
        var request = new CalculateRebateRequest { Volume = (decimal)volume };

        var result = _strategy.Calculate(rebate, product, request);

        Assert.Equal((decimal)expected, result);
    }
}
