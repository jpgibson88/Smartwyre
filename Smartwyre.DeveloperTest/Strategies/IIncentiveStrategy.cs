using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public interface IIncentiveStrategy
{
    IncentiveType Type { get; }
    
    bool IsValid (Rebate rebate, Product product, CalculateRebateRequest request);
    
    decimal Calculate (Rebate rebate, Product product,  CalculateRebateRequest request);
}