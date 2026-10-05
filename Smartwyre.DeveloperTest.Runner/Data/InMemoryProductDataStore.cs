using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner.Data;

// Sample data so the runner can demonstrate both supported and unsupported incentives
public class InMemoryProductDataStore : IProductDataStore
{
    private readonly Dictionary<string, Product> _products = new(StringComparer.OrdinalIgnoreCase)
    {
        ["widget"] = new Product
        {
            Id = 1,
            Identifier = "widget",
            Price = 20m,
            Uom = "each",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
                                  | SupportedIncentiveType.AmountPerUom
                                  | SupportedIncentiveType.FixedRateRebate
        },
        ["gadget"] = new Product
        {
            Id = 2,
            Identifier = "gadget",
            Price = 5m,
            Uom = "each",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        },
    };

    public IEnumerable<Product> All => _products.Values;

    public Product GetProduct(string productIdentifier) =>
        _products.GetValueOrDefault(productIdentifier);
}
