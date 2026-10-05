using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class AmountPerUomStrategy : IncentiveStrategyBase
{
    public override IncentiveType Type => IncentiveType.AmountPerUom;
    
    protected override bool Validate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate.Amount == 0 || request.Volume == 0)
        {
            return false;
        }   

        return true;
    }

    public override decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return rebate.Amount * request.Volume;
    }
}