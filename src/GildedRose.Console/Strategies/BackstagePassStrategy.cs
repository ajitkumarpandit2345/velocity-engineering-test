using System;

namespace GildedRose.Console.Strategies;

/// <summary>
/// Strategy for Backstage Passes - increases in quality as concert approaches, drops to 0 after.
/// </summary>
public class BackstagePassStrategy : IItemUpdateStrategy
{
    private const string ItemName = "Backstage passes to a TAFKAL80ETC concert";
    private const int MaxQuality = 50;

    public bool CanHandle(Item item) => item.Name == ItemName;

    public void Update(Item item)
    {
        item.SellIn--;

        // Concert passed: quality drops directly to 0
        if (item.SellIn < 0)
        {
            item.Quality = 0;
        }
        // <= 5 days remaining: increases by 3
        else if (item.SellIn < 5)
        {
            item.Quality = Math.Min(MaxQuality, item.Quality + 3);
        }
        // <= 10 days remaining: increases by 2
        else if (item.SellIn < 10)
        {
            item.Quality = Math.Min(MaxQuality, item.Quality + 2);
        }
        // > 10 days remaining: standard increase of 1
        else
        {
            item.Quality = Math.Min(MaxQuality, item.Quality + 1);
        }
    }
}
