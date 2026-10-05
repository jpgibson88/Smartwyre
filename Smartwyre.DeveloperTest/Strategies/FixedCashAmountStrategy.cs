using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class FixedCashAmountStrategy : IncentiveStrategyBase
{
    public override IncentiveType Type => IncentiveType.FixedCashAmount;
    
    protected override bool Validate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate.Amount == 0)
        {
            return false;
        }

        return true;
    }

    public override decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return rebate.Amount;
    }
}