using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Strategies;

public class FixedCashAmountStrategyTests
{
    private readonly FixedCashAmountStrategy _strategy = new();

    private static Rebate ValidRebate() => new() { Incentive = IncentiveType.FixedCashAmount, Amount = 100m };

    private static Product ValidProduct() => new() { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

    private static CalculateRebateRequest ValidRequest() => new() { Volume = 10m };

    [Fact]
    public void Type_IsFixedCashAmount()
    {
        Assert.Equal(IncentiveType.FixedCashAmount, _strategy.Type);
    }

    [Fact]
    public void IsValid_AllInputsValid_ReturnsTrue()
    {
        Assert.True(_strategy.IsValid(ValidRebate(), ValidProduct(), ValidRequest()));
    }

    [Fact]
    public void IsValid_ZeroVolume_ReturnsTrue_BecauseVolumeIsNotUsedForFixedCash()
    {
        var request = ValidRequest();
        request.Volume = 0m;

        Assert.True(_strategy.IsValid(ValidRebate(), ValidProduct(), request));
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
    public void IsValid_ZeroRebateAmount_ReturnsFalse()
    {
        var rebate = ValidRebate();
        rebate.Amount = 0m;

        Assert.False(_strategy.IsValid(rebate, ValidProduct(), ValidRequest()));
    }

    [Fact]
    public void Calculate_ReturnsRebateAmountRegardlessOfVolume()
    {
        var request = new CalculateRebateRequest { Volume = 999m };

        var result = _strategy.Calculate(ValidRebate(), ValidProduct(), request);

        Assert.Equal(100m, result);
    }
}
