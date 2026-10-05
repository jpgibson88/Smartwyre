using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class FixedRateRebateStrategy : IncentiveStrategyBase
{
    public override IncentiveType Type => IncentiveType.FixedRateRebate;
    
    protected override bool Validate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
        {
            return false;
        }

        return true;
    }

    public override decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return product.Price * rebate.Percentage * request.Volume;
    }
}