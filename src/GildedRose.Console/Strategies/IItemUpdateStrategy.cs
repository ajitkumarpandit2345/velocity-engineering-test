namespace GildedRose.Console.Strategies;

/// <summary>
/// Strategy interface defining the contract for updating an item's daily status.
/// Implements the Strategy Pattern to adhere to Open/Closed Principle (OCP).
/// </summary>
public interface IItemUpdateStrategy
{
    // Evaluates whether this strategy can handle the given item
    bool CanHandle(Item item);

    // Applies daily quality and sell-in updates to the item
    void Update(Item item);
}
