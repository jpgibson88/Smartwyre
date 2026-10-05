using System;
using System.Collections.Generic;
using System.Linq;

namespace Smartwyre.DeveloperTest.Strategies;

public static class IncentiveStrategyDiscovery
{
    // Finds every concrete IIncentiveStrategy in this assembly so new strategies are picked up without extra registration
    public static IEnumerable<Type> FindStrategyTypes() =>
        typeof(IIncentiveStrategy).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IIncentiveStrategy).IsAssignableFrom(type));
}
