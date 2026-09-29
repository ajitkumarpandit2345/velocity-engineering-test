using System.Collections.Generic;

namespace GildedRose.Console;

public class Program
{
    // Converted public field to auto-property for proper encapsulation and interface compliance
    public IList<Item> Items { get; set; } = new List<Item>();

    public static void Main(string[] args)
    {
        System.Console.WriteLine("OMGHAI!");

        // Normalized object initializer indentation to standard C# Allman style
        var app = new Program
        {
            Items = new List<Item>
            {
                new Item { Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20 },
                new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 },
                new Item { Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7 },
                new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 15,
                    Quality = 20
                },
                new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 6 }
            }
        };

        app.UpdateQuality();

        System.Console.ReadKey();
    }

    // Delegated to GildedRoseService to fulfill Single Responsibility Principle (entry point vs domain logic)
    public void UpdateQuality()
    {
        new GildedRoseService(Items).UpdateQuality();
    }
}