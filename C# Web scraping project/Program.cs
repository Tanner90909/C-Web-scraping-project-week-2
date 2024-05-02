using CSharp_Web_scraping_project.Models;
using PuppeteerSharp;
using System;

class Program
{
    static void Main(string[] args)
    {
        var testCard = new Card
        {
            Title = "George Humphries Card",
            Rarity = "Ultra Rare",
            QuantityOfListings = 1,
            MarketPrice = 500000.99m
        };

        using (var context = new CardDbContext())
        {
            context.Database.EnsureCreated();
            context.tblCardDetails.Add(testCard);
            context.SaveChanges();
        }

        Console.WriteLine("Card added to the database successfully");
    }
}