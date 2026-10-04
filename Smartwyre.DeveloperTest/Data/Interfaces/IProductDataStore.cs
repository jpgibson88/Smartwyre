using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data.Interfaces;

public interface IProductDataStore
{
    public Product GetProduct(string productIdentifier);
}