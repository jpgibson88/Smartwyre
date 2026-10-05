using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class FixedCashAmountStrategy : IIncentiveStrategy
{
    public IncentiveType Type => IncentiveType.FixedCashAmount;
    
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
        
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
        {
            return false;
        }
        
        if (rebate.Amount == 0)
        {
            return false;
        }

        return true;
    }

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return rebate.Amount;
    }
}