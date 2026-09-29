using System;
using System.Collections.Generic;

namespace GildedRose.Console.Strategies;

/// <summary>
/// Factory and resolver for item update strategies.
/// Enables adding new supplier or category strategies without changing existing update logic.
/// </summary>
public static class ItemStrategyFactory
{
    private static readonly IItemUpdateStrategy DefaultStrategy = new StandardItemStrategy();

    // Specific strategies evaluated in order; the first match wins
    private static readonly List<IItemUpdateStrategy> SpecificStrategies = new()
    {
        new SulfurasStrategy(),
        new AgedBrieStrategy(),
        new BackstagePassStrategy(),
        new ConjuredItemStrategy()
    };

    /// <summary>
    /// Resolves the appropriate update strategy for an item.
    /// Falls back to StandardItemStrategy if no specific strategy matches.
    /// </summary>
    public static IItemUpdateStrategy GetStrategy(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);

        foreach (var strategy in SpecificStrategies)
        {
            if (strategy.CanHandle(item))
            {
                return strategy;
            }
        }

        return DefaultStrategy;
    }
}
