using System;

namespace GildedRose.Console.Strategies;

/// <summary>
/// Strategy for Conjured items - degrades in quality twice as fast as standard items.
/// </summary>
public class ConjuredItemStrategy : IItemUpdateStrategy
{
    private const string ConjuredPrefix = "Conjured";
    private const int MinQuality = 0;

    public bool CanHandle(Item item) =>
        item.Name != null && item.Name.StartsWith(ConjuredPrefix, StringComparison.Ordinal);

    public void Update(Item item)
    {
        item.SellIn--;

        // Degrades by 2 before expiration, 4 after expiration
        int degradation = item.SellIn < 0 ? 4 : 2;
        item.Quality = Math.Max(MinQuality, item.Quality - degradation);
    }
}
