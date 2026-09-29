using System;

namespace GildedRose.Console.Strategies;

/// <summary>
/// Strategy for Aged Brie - increases in quality the older it gets.
/// </summary>
public class AgedBrieStrategy : IItemUpdateStrategy
{
    private const string ItemName = "Aged Brie";
    private const int MaxQuality = 50;

    public bool CanHandle(Item item) => item.Name == ItemName;

    public void Update(Item item)
    {
        item.SellIn--;

        // Aged Brie increases in quality; rate doubles once past sell date
        int increase = item.SellIn < 0 ? 2 : 1;
        item.Quality = Math.Min(MaxQuality, item.Quality + increase);
    }
}
