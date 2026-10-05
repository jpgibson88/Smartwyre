using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore, IEnumerable<IIncentiveStrategy> incentiveStrategies) : IRebateService
{
    private readonly Dictionary<IncentiveType, IIncentiveStrategy> _incentiveStrategies = BuildLookup(incentiveStrategies);

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();

        if (rebate == null || !_incentiveStrategies.TryGetValue(rebate.Incentive, out var incentiveStrategy))
        {
            result.Success = false;
            return result;
        }

        result.Success = incentiveStrategy.IsValid(rebate, product, request);

        if (result.Success)
        {
            var amount = incentiveStrategy.Calculate(rebate, product, request);
            rebateDataStore.StoreCalculationResult(rebate, amount);
        }

        return result;
    }

    // Each incentive type must map to exactly one strategy
    private static Dictionary<IncentiveType, IIncentiveStrategy> BuildLookup(IEnumerable<IIncentiveStrategy> strategies)
    {
        var lookup = new Dictionary<IncentiveType, IIncentiveStrategy>();

        foreach (var strategy in strategies)
        {
            if (!lookup.TryAdd(strategy.Type, strategy))
            {
                throw new InvalidOperationException(
                    $"Multiple strategies registered for incentive type {strategy.Type}: " +
                    $"{lookup[strategy.Type].GetType().Name} and {strategy.GetType().Name}.");
            }
        }

        return lookup;
    }
}
