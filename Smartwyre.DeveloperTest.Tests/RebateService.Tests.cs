using System;
using Moq;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    [Fact]
    public void Test1()
    {
        var productDataStoreMock = new Mock<IProductDataStore>();
        productDataStoreMock.Setup(mock => mock.GetProduct("1"))
            .Returns(new Product()
                {
                    Identifier = "1",
                    Id = 1,
                    Price = 19.99m,
                }
            );
        var rebateDataStoreMock = new Mock<IRebateDataStore>();
        var service = new RebateService(rebateDataStoreMock.Object, productDataStoreMock.Object);
    }
}
