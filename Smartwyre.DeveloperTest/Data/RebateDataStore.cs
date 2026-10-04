using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore
{
    private Dictionary<string, Rebate> _rebates;

    public RebateDataStore()
    {
        _rebates = new  Dictionary<string, Rebate>();
        
        // TODO: Initialize rebates with rebate list here.
        // Maybe read in a csv list 
    }
    
    public Rebate GetRebate(string rebateIdentifier)
    {
        // Access database to retrieve account, code removed for brevity
        return _rebates[rebateIdentifier];
    }

    public void StoreCalculationResult(Rebate account, decimal rebateAmount)
    {
        // Update account in database, code removed for brevity
        GetRebate(account.Identifier).Amount = rebateAmount;
    }
}
