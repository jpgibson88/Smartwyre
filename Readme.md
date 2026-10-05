# Smartwyre Developer Test Instructions

_________
## Solution Notes

### Changes
- Each incentive type is validated and calculated by its own strategy class (`IIncentiveStrategy`), so `RebateService` only looks up data, picks the strategy for the rebate's incentive type and stores the result
- Checks shared by every strategy (rebate and product exist, product supports the incentive) live in `IncentiveStrategyBase`
- All dependencies (data stores and strategies) are injected through constructors
- Strategies are discovered automatically and registered with the DI container in the Runner
- Unit tests cover `RebateService` (with mocked strategies), each strategy, the full flow with the real strategies, and strategy discovery

### Running the Runner
```
dotnet run --project Smartwyre.DeveloperTest.Runner -- <rebateId> <productId> <volume>
```
The Runner uses in-memory sample data (`Smartwyre.DeveloperTest.Runner/Data`) because the original data stores are placeholders that always return an empty rebate/product. Run it with no arguments to list the sample rebates and products. For example:
- `cash-100 widget 1`: FixedCashAmount, stores 100
- `uom-2 widget 10`: AmountPerUom, stores 20
- `rate-10 widget 10`: FixedRateRebate, stores 20
- `rate-10 gadget 10`: not valid, because gadget doesn't support FixedRateRebate

Exit codes: `0` succeeded, `1` bad input or error, `2` calculation not valid.

### To add a new incentive type
1. Add the new type to both the `IncentiveType` and `SupportedIncentiveType` enums in `Smartwyre.DeveloperTest.Types` (use the same name in both)
2. Add a strategy class that inherits from `IncentiveStrategyBase` in `Smartwyre.DeveloperTest.Strategies`

The strategy is registered automatically. `IncentiveStrategyDiscoveryTests` fails if an incentive type has no strategy, has more than one, or uses a mismatched `SupportedIncentiveType`.
_________


You have been selected to complete our candidate coding exercise. Please follow the directions in this readme.

Clone, **DO NOT FORK**, this repository to your account on the online Git resource of your choosing (GitHub, BitBucket, GitLab, etc.). Your solution should retain previous commit history and you should utilize best practices for committing your changes to the repository.

You are welcome to use whatever tools you normally would when coding — including documentation, libraries, frameworks, or AI tools (such as ChatGPT or Copilot).

However, it is important that you fully understand your solution. As part of the interview process, we will review your code with you in detail. You should be able to:

- Explain the design choices you made.
- Walk us through how your solution works.
- Make modifications or extensions to your code during the review.

Please note: if your submission appears to have been generated entirely by an AI agent or another third party, without your own understanding or contribution, it will not meet our evaluation criteria.

# The Exercise

In the 'RebateService.cs' file you will find a method for calculating a rebate. At a high level the steps for calculating a rebate are:

 1. Lookup the rebate that the request is being made against.
 2. Lookup the product that the request is being made against.
 2. Check that the rebate and request are valid to calculate the incentive type rebate.
 3. Store the rebate calculation.

What we'd like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principles
 - Testability
 - Readability
 - Currently there are 3 known incentive types. In the future the business will want to add many more incentive types. Your solution should make it easy for developers to add new incentive types in the future.

We’d also like you to 
 - Add some unit tests to the Smartwyre.DeveloperTest.Tests project to show how you would test the code that you’ve produced 
 - Run the RebateService from the Smartwyre.DeveloperTest.Runner console application accepting inputs (either via command line arguments or via prompts is fine)

The only specific "rules" are:

- The solution must build
- All tests must pass

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around 1 hour completing the exercise.

Feel free to use code comments to describe your changes. You are also welcome to update this readme with any important details for us to consider.

Once you have completed the exercise either ensure your repository is available publicly or contact the hiring manager to set up a private share.

