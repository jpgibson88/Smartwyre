using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore, IEnumerable<IIncentiveStrategy> incentiveStrategies) : IRebateService
{
    private Dictionary<IncentiveType, IIncentiveStrategy> _incentiveStrategies = incentiveStrategies.ToDictionary(strategy => strategy.Type, strategy => strategy);
    
    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = productDataStore.GetProduct(request.ProductIdentifier);

        if (!_incentiveStrategies.TryGetValue(rebate.Incentive, out var incentiveStrategy))
        {
            throw new Exception("Unsupported incentive");
        }
        
        var result = new CalculateRebateResult
        {
            Success = incentiveStrategy.IsValid(rebate, product, request)
        };

        if (result.Success)
        {
            var amount = incentiveStrategy.Calculate(rebate, product, request);
            rebateDataStore.StoreCalculationResult(rebate, amount);
        }

        return result;
    }
}
