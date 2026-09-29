using System.Collections.Generic;
using GildedRose.Console;
using Xunit;

namespace GildedRose.Tests;

public class TestAssemblyTests
{
    [Fact]
    public void StandardItem_BeforeSellDate_DecreasesQualityAndSellInByOne()
    {
        var items = new List<Item> { new Item { Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(9, items[0].SellIn);
        Assert.Equal(19, items[0].Quality);
    }

    [Fact]
    public void StandardItem_AfterSellDate_DegradesTwiceAsFast()
    {
        var items = new List<Item> { new Item { Name = "Standard Item", SellIn = 0, Quality = 10 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(-1, items[0].SellIn);
        Assert.Equal(8, items[0].Quality);
    }

    [Fact]
    public void StandardItem_QualityNeverNegative()
    {
        var items = new List<Item> { new Item { Name = "Standard Item", SellIn = 5, Quality = 0 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void AgedBrie_IncreasesInQuality()
    {
        var items = new List<Item> { new Item { Name = "Aged Brie", SellIn = 2, Quality = 10 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(1, items[0].SellIn);
        Assert.Equal(11, items[0].Quality);
    }

    [Fact]
    public void AgedBrie_QualityNeverExceedsFifty()
    {
        var items = new List<Item> { new Item { Name = "Aged Brie", SellIn = 2, Quality = 50 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(50, items[0].Quality);
    }

    [Fact]
    public void AgedBrie_AfterSellDate_IncreasesQualityTwiceAsFast()
    {
        var items = new List<Item> { new Item { Name = "Aged Brie", SellIn = 0, Quality = 10 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(-1, items[0].SellIn);
        Assert.Equal(12, items[0].Quality);
    }

    [Fact]
    public void Sulfuras_NeverDecreasesInQualityOrSellIn()
    {
        var items = new List<Item> { new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 5, Quality = 80 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(5, items[0].SellIn);
        Assert.Equal(80, items[0].Quality);
    }

    [Theory]
    [InlineData(15, 20, 21)] // > 10 days: increases by 1
    [InlineData(10, 20, 22)] // <= 10 days: increases by 2
    [InlineData(6, 20, 22)]  // <= 10 days: increases by 2
    [InlineData(5, 20, 23)]  // <= 5 days: increases by 3
    [InlineData(1, 20, 23)]  // <= 5 days: increases by 3
    [InlineData(0, 20, 0)]   // After concert: drops to 0
    public void BackstagePasses_IncreaseQualityBasedOnSellIn(int sellIn, int initialQuality, int expectedQuality)
    {
        var items = new List<Item>
        {
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = sellIn, Quality = initialQuality }
        };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(expectedQuality, items[0].Quality);
    }

    [Fact]
    public void ConjuredItem_BeforeSellDate_DegradesTwiceAsFastAsNormal()
    {
        var items = new List<Item> { new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 6 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(2, items[0].SellIn);
        Assert.Equal(4, items[0].Quality); // Drops by 2 instead of 1
    }

    [Fact]
    public void ConjuredItem_AfterSellDate_DegradesTwiceAsFastAsNormalExpired()
    {
        var items = new List<Item> { new Item { Name = "Conjured Mana Cake", SellIn = 0, Quality = 6 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(-1, items[0].SellIn);
        Assert.Equal(2, items[0].Quality); // Drops by 4 instead of 2
    }

    [Fact]
    public void ConjuredItem_QualityNeverNegative()
    {
        var items = new List<Item> { new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 1 } };
        var app = new Program { Items = items };

        app.UpdateQuality();

        Assert.Equal(0, items[0].Quality);
    }
}