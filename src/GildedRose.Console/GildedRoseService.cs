using System;
using System.Collections.Generic;
using GildedRose.Console.Strategies;

namespace GildedRose.Console;

/// <summary>
/// Encapsulates inventory evaluation rules, separating domain logic from application entry point.
/// Delegates item processing to polymorphic strategies via the Strategy Pattern.
/// </summary>
public class GildedRoseService
{
    private readonly IList<Item> _items;

    public GildedRoseService(IList<Item> items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
    }

    /// <summary>
    /// Evaluates daily updates for all inventory items.
    /// </summary>
    public void UpdateQuality()
    {
        foreach (var item in _items)
        {
            // Resolves the strategy dynamically, isolating each item category's business rules
            var strategy = ItemStrategyFactory.GetStrategy(item);
            strategy.Update(item);
        }
    }
}
