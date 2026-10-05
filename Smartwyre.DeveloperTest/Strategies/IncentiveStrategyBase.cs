using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public abstract class IncentiveStrategyBase : IIncentiveStrategy
{
    public abstract IncentiveType Type { get; }
    
    // Shared base validation for all strategies
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
        
        return Validate(rebate, product, request);
    }
    
    protected abstract bool Validate(Rebate rebate, Product product, CalculateRebateRequest request);

    public abstract decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request);
}