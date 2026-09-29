namespace GildedRose.Console;

/// <summary>
/// Represents an inventory item in the Gilded Rose inn.
/// Extracted to its own file to adhere to the one-type-per-file C# convention.
/// </summary>
public class Item
{
    public string Name { get; set; } = string.Empty;

    public int SellIn { get; set; }

    public int Quality { get; set; }
}
