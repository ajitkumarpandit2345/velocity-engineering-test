namespace GildedRose.Console.Strategies;

/// <summary>
/// Strategy for Sulfuras - legendary item that never ages or changes quality.
/// </summary>
public class SulfurasStrategy : IItemUpdateStrategy
{
    private const string ItemName = "Sulfuras, Hand of Ragnaros";

    public bool CanHandle(Item item) => item.Name == ItemName;

    public void Update(Item item)
    {
        // Legendary item: neither SellIn decreases nor Quality changes
    }
}
