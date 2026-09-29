using System;

namespace GildedRose.Console.Strategies;

/// <summary>
/// Default fallback strategy for standard items.
/// </summary>
public class StandardItemStrategy : IItemUpdateStrategy
{
    private const int MinQuality = 0;

    // Standard strategy acts as the default fallback for any unhandled item
    public bool CanHandle(Item item) => true;

    public void Update(Item item)
    {
        item.SellIn--;

        // Degrades by 1 before expiration, 2 after expiration
        int degradation = item.SellIn < 0 ? 2 : 1;
        item.Quality = Math.Max(MinQuality, item.Quality - degradation);
    }
}
