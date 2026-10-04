using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data.Interfaces;

public interface IRebateDataStore
{
    public Rebate GetRebate(string rebateIdentifier);

    public void StoreCalculationResult(Rebate account, decimal rebateAmount);
}