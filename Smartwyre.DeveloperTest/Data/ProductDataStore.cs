using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore
{
    private Dictionary<string, Product> _products;

    public ProductDataStore()
    {
        _products = new Dictionary<string, Product>();
        
        // TODO: initialize products
    }
    
    public Product GetProduct(string productIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        return _products[productIdentifier];
    }
}
