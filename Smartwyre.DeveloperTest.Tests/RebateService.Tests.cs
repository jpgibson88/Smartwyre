using System;
using System.Collections.Generic;
using Moq;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private const string RebateId = "rebate-1";
    private const string ProductId = "product-1";

    private readonly Mock<IRebateDataStore> _rebateDataStore = new();
    private readonly Mock<IProductDataStore> _productDataStore = new();

    private static CalculateRebateRequest Request(decimal volume = 10m) => new()
    {
        RebateIdentifier = RebateId,
        ProductIdentifier = ProductId,
        Volume = volume
    };

    /// <summary>Creates a strategy mock for the given incentive type with configurable validity and amount.</summary>
    private static Mock<IIncentiveStrategy> StrategyMock(IncentiveType type, bool isValid, decimal amount = 0m)
    {
        var strategy = new Mock<IIncentiveStrategy>();
        strategy.Setup(s => s.Type).Returns(type);
        strategy.Setup(s => s.IsValid(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns(isValid);
        strategy.Setup(s => s.Calculate(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns(amount);
        return strategy;
    }

    private RebateService CreateService(params IIncentiveStrategy[] strategies) =>
        new(_rebateDataStore.Object, _productDataStore.Object, strategies);

    private (Rebate rebate, Product product) SetupData(IncentiveType incentive)
    {
        var rebate = new Rebate { Identifier = RebateId, Incentive = incentive };
        var product = new Product { Identifier = ProductId };
        _rebateDataStore.Setup(s => s.GetRebate(RebateId)).Returns(rebate);
        _productDataStore.Setup(s => s.GetProduct(ProductId)).Returns(product);
        return (rebate, product);
    }

    [Fact]
    public void Calculate_ValidStrategy_ReturnsSuccessAndStoresCalculatedAmount()
    {
        var (rebate, _) = SetupData(IncentiveType.FixedCashAmount);
        var strategy = StrategyMock(IncentiveType.FixedCashAmount, isValid: true, amount: 42m);
        var service = CreateService(strategy.Object);

        var result = service.Calculate(Request());

        Assert.True(result.Success);
        _rebateDataStore.Verify(s => s.StoreCalculationResult(rebate, 42m), Times.Once);
    }

    [Fact]
    public void Calculate_InvalidStrategy_ReturnsFailureAndDoesNotStoreOrCalculate()
    {
        SetupData(IncentiveType.FixedCashAmount);
        var strategy = StrategyMock(IncentiveType.FixedCashAmount, isValid: false);
        var service = CreateService(strategy.Object);

        var result = service.Calculate(Request());

        Assert.False(result.Success);
        _rebateDataStore.Verify(
            s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
        strategy.Verify(
            s => s.Calculate(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()),
            Times.Never);
    }

    [Fact]
    public void Calculate_LooksUpRebateAndProductUsingRequestIdentifiers()
    {
        SetupData(IncentiveType.AmountPerUom);
        var service = CreateService(StrategyMock(IncentiveType.AmountPerUom, isValid: true).Object);

        service.Calculate(Request());

        _rebateDataStore.Verify(s => s.GetRebate(RebateId), Times.Once);
        _productDataStore.Verify(s => s.GetProduct(ProductId), Times.Once);
    }

    [Fact]
    public void Calculate_RebateNotFound_ReturnsFailureAndDoesNotStore()
    {
        _rebateDataStore.Setup(s => s.GetRebate(RebateId)).Returns((Rebate)null);
        _productDataStore.Setup(s => s.GetProduct(ProductId)).Returns(new Product { Identifier = ProductId });
        var strategy = StrategyMock(IncentiveType.FixedRateRebate, isValid: true, amount: 10m);
        var service = CreateService(strategy.Object);

        var result = service.Calculate(Request());

        Assert.False(result.Success);
        strategy.Verify(
            s => s.IsValid(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()),
            Times.Never);
        _rebateDataStore.Verify(
            s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_MultipleStrategies_UsesOnlyTheOneMatchingTheRebateIncentive()
    {
        SetupData(IncentiveType.AmountPerUom);
        var matching = StrategyMock(IncentiveType.AmountPerUom, isValid: true, amount: 5m);
        var other = StrategyMock(IncentiveType.FixedRateRebate, isValid: true, amount: 99m);
        var service = CreateService(other.Object, matching.Object);

        service.Calculate(Request());

        matching.Verify(
            s => s.Calculate(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()),
            Times.Once);
        other.Verify(
            s => s.IsValid(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()),
            Times.Never);
        _rebateDataStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), 5m), Times.Once);
    }

    [Fact]
    public void Calculate_PassesRebateProductAndRequestToStrategy()
    {
        var (rebate, product) = SetupData(IncentiveType.AmountPerUom);
        var strategy = StrategyMock(IncentiveType.AmountPerUom, isValid: true, amount: 1m);
        var request = Request(7m);
        var service = CreateService(strategy.Object);

        service.Calculate(request);

        strategy.Verify(s => s.IsValid(rebate, product, request), Times.Once);
        strategy.Verify(s => s.Calculate(rebate, product, request), Times.Once);
    }

    [Fact]
    public void Calculate_NoStrategyForIncentive_ReturnsFailure()
    {
        SetupData(IncentiveType.FixedRateRebate);
        var service = CreateService(StrategyMock(IncentiveType.AmountPerUom, isValid: true).Object);

        var result = service.Calculate(Request());

        Assert.False(result.Success);
        _rebateDataStore.Verify(
            s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Constructor_TwoStrategiesForSameIncentive_Throws()
    {
        var first = StrategyMock(IncentiveType.AmountPerUom, isValid: true).Object;
        var second = StrategyMock(IncentiveType.AmountPerUom, isValid: true).Object;

        var exception = Assert.Throws<InvalidOperationException>(() => CreateService(first, second));

        Assert.Contains(nameof(IncentiveType.AmountPerUom), exception.Message);
    }

    /// <summary>
    /// Wires the real strategies together (only the data stores are mocked) to check the
    /// whole flow end to end for each incentive type.
    /// </summary>
    [Theory]
    [MemberData(nameof(EndToEndCases))]
    public void Calculate_WithRealStrategies_StoresExpectedAmount(
        IncentiveType incentive,
        SupportedIncentiveType supported,
        decimal rebateAmount,
        decimal percentage,
        decimal price,
        decimal volume,
        decimal expectedAmount)
    {
        var rebate = new Rebate
        {
            Identifier = RebateId,
            Incentive = incentive,
            Amount = rebateAmount,
            Percentage = percentage
        };
        var product = new Product { Identifier = ProductId, Price = price, SupportedIncentives = supported };
        _rebateDataStore.Setup(s => s.GetRebate(RebateId)).Returns(rebate);
        _productDataStore.Setup(s => s.GetProduct(ProductId)).Returns(product);
        var service = CreateService(
            new FixedRateRebateStrategy(), new AmountPerUomStrategy(), new FixedCashAmountStrategy());

        var result = service.Calculate(Request(volume));

        Assert.True(result.Success);
        _rebateDataStore.Verify(s => s.StoreCalculationResult(rebate, expectedAmount), Times.Once);
    }

    public static IEnumerable<object[]> EndToEndCases()
    {
        // incentive, supported, rebate amount, percentage, price, volume, expected
        yield return new object[]
            { IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount, 100m, 0m, 0m, 0m, 100m };
        yield return new object[]
            { IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, 2m, 0m, 0m, 10m, 20m };
        yield return new object[]
            { IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, 0m, 0.1m, 20m, 10m, 20m };
    }

    [Fact]
    public void Calculate_WithRealStrategies_ProductNotSupportingIncentive_ReturnsFailure()
    {
        var rebate = new Rebate { Identifier = RebateId, Incentive = IncentiveType.FixedCashAmount, Amount = 100m };
        var product = new Product { Identifier = ProductId, SupportedIncentives = SupportedIncentiveType.AmountPerUom };
        _rebateDataStore.Setup(s => s.GetRebate(RebateId)).Returns(rebate);
        _productDataStore.Setup(s => s.GetProduct(ProductId)).Returns(product);
        var service = CreateService(new FixedCashAmountStrategy());

        var result = service.Calculate(Request());

        Assert.False(result.Success);
        _rebateDataStore.Verify(
            s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }
}
