using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner.Data;

// Sample data so the runner can demonstrate a successful calculation for each incentive type
public class InMemoryRebateDataStore : IRebateDataStore
{
    private readonly Dictionary<string, Rebate> _rebates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["cash-100"] = new Rebate { Identifier = "cash-100", Incentive = IncentiveType.FixedCashAmount, Amount = 100m },
        ["uom-2"] = new Rebate { Identifier = "uom-2", Incentive = IncentiveType.AmountPerUom, Amount = 2m },
        ["rate-10"] = new Rebate { Identifier = "rate-10", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m },
    };

    public IEnumerable<Rebate> All => _rebates.Values;

    public Rebate GetRebate(string rebateIdentifier) =>
        _rebates.GetValueOrDefault(rebateIdentifier);

    public void StoreCalculationResult(Rebate account, decimal rebateAmount)
    {
        Console.WriteLine($"Stored rebate amount {rebateAmount} for rebate '{account.Identifier}'.");
    }
}
