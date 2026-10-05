using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class FixedRateRebateStrategy : IIncentiveStrategy
{
    public IncentiveType Type => IncentiveType.FixedRateRebate;
    
    public bool IsValid(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate == null)
        {
            return false;
        }
        
        if (product == null)
        {
            return false;
        }
        
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
        {
            return false;
        }
        
        if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
        {
            return false;
        }

        return true;
    }
    
    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return product.Price * rebate.Percentage * request.Volume;
    }
}