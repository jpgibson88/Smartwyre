using System;
using System.Linq;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Strategies;

/// <summary>
/// Guards the steps for adding a new incentive type: every IncentiveType needs exactly one
/// strategy, and that strategy must check the matching SupportedIncentiveType flag.
/// </summary>
public class IncentiveStrategyDiscoveryTests
{
    private static IIncentiveStrategy[] DiscoveredStrategies() =>
        IncentiveStrategyDiscovery.FindStrategyTypes()
            .Select(type => (IIncentiveStrategy)Activator.CreateInstance(type))
            .ToArray();

    [Fact]
    public void FindStrategyTypes_ExcludesAbstractBaseClass()
    {
        Assert.DoesNotContain(typeof(IncentiveStrategyBase), IncentiveStrategyDiscovery.FindStrategyTypes());
    }

    [Fact]
    public void EveryIncentiveType_HasExactlyOneStrategy()
    {
        var strategyTypes = DiscoveredStrategies().Select(s => s.Type).ToArray();

        foreach (var incentive in Enum.GetValues<IncentiveType>())
        {
            Assert.Single(strategyTypes, type => type == incentive);
        }
    }

    [Fact]
    public void EveryStrategy_RequiresTheSupportedIncentiveWithTheSameName()
    {
        foreach (var strategy in DiscoveredStrategies().OfType<IncentiveStrategyBase>())
        {
            Assert.Equal(strategy.Type.ToString(), strategy.RequiredSupport.ToString());
        }
    }
}
