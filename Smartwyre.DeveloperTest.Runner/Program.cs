using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Runner.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static int Main(string[] args)
    {
        var request = BuildRequestFromArgs(args);

        if (request == null)
        {
            PrintUsage();
            return 1;
        }

        var builder = Host.CreateApplicationBuilder();

        // In-memory sample data; swap for RebateDataStore/ProductDataStore once they talk to a real database
        builder.Services.AddSingleton<IRebateDataStore, InMemoryRebateDataStore>();
        builder.Services.AddSingleton<IProductDataStore, InMemoryProductDataStore>();

        // Every IIncentiveStrategy in Smartwyre.DeveloperTest is registered automatically
        foreach (var strategyType in IncentiveStrategyDiscovery.FindStrategyTypes())
        {
            builder.Services.AddTransient(typeof(IIncentiveStrategy), strategyType);
        }

        builder.Services.AddTransient<IRebateService, RebateService>();

        using var app = builder.Build();
        var rebateService = app.Services.GetRequiredService<IRebateService>();

        try
        {
            var result = rebateService.Calculate(request);
            Console.WriteLine($"Rebate calculation {(result.Success ? "succeeded" : "was not valid")}.");
            return result.Success ? 0 : 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Calculation failed: {ex.Message}");
            return 1;
        }
    }

    private static CalculateRebateRequest BuildRequestFromArgs(string[] args)
    {
        if (args.Length < 3)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(args[0]) ||
            string.IsNullOrWhiteSpace(args[1]) ||
            !TryParseVolume(args[2], out var volume))
        {
            return null;
        }

        return new CalculateRebateRequest
        {
            RebateIdentifier = args[0],
            ProductIdentifier = args[1],
            Volume = volume
        };
    }

    private static bool TryParseVolume(string input, out decimal volume)
    {
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out volume)
               && volume >= 0;
    }

    private static void PrintUsage()
    {
        Console.Error.WriteLine("Usage: Runner <rebateId> <productId> <volume>");
        Console.Error.WriteLine();
        Console.Error.WriteLine("Sample rebates:");
        foreach (var rebate in new InMemoryRebateDataStore().All)
        {
            Console.Error.WriteLine($"  {rebate.Identifier,-10} {rebate.Incentive}");
        }

        Console.Error.WriteLine("Sample products:");
        foreach (var product in new InMemoryProductDataStore().All)
        {
            Console.Error.WriteLine($"  {product.Identifier,-10} price {product.Price}, supports {product.SupportedIncentives}");
        }
    }
}
