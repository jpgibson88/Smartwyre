using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Strategies;

public class AmountPerUomStrategyTests
{
    private readonly AmountPerUomStrategy _strategy = new();

    private static Rebate ValidRebate() => new() { Incentive = IncentiveType.AmountPerUom, Amount = 2m };

    private static Product ValidProduct() => new() { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

    private static CalculateRebateRequest ValidRequest() => new() { Volume = 10m };

    [Fact]
    public void Type_IsAmountPerUom()
    {
        Assert.Equal(IncentiveType.AmountPerUom, _strategy.Type);
    }

    [Fact]
    public void IsValid_AllInputsValid_ReturnsTrue()
    {
        Assert.True(_strategy.IsValid(ValidRebate(), ValidProduct(), ValidRequest()));
    }

    [Fact]
    public void IsValid_ProductSupportsSeveralIncentivesIncludingThisOne_ReturnsTrue()
    {
        var product = ValidProduct();
        product.SupportedIncentives = SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedCashAmount;

        Assert.True(_strategy.IsValid(ValidRebate(), product, ValidRequest()));
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
        product.SupportedIncentives = SupportedIncentiveType.FixedCashAmount;

        Assert.False(_strategy.IsValid(ValidRebate(), product, ValidRequest()));
    }

    [Fact]
    public void IsValid_ZeroRebateAmount_ReturnsFalse()
    {
        var rebate = ValidRebate();
        rebate.Amount = 0m;

        Assert.False(_strategy.IsValid(rebate, ValidProduct(), ValidRequest()));
    }

    [Fact]
    public void IsValid_ZeroVolume_ReturnsFalse()
    {
        var request = ValidRequest();
        request.Volume = 0m;

        Assert.False(_strategy.IsValid(ValidRebate(), ValidProduct(), request));
    }

    [Theory]
    [InlineData(2, 10, 20)]
    [InlineData(0.5, 3, 1.5)]
    [InlineData(1.25, 4, 5)]
    public void Calculate_ReturnsAmountMultipliedByVolume(double amount, double volume, double expected)
    {
        var rebate = ValidRebate();
        rebate.Amount = (decimal)amount;
        var request = new CalculateRebateRequest { Volume = (decimal)volume };

        var result = _strategy.Calculate(rebate, ValidProduct(), request);

        Assert.Equal((decimal)expected, result);
    }
}
