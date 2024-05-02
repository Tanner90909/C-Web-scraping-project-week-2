﻿using CSharp_Web_scraping_project.Models;
using PuppeteerSharp;
using System;

class Program
{
    static void Main(string[] args)
    {
        var testCard = new Card
        {
            Title = "Test Card",
            Rarity = "Common",
            QuantityOfListings = 10,
            MarketPrice = 5.99m
            // No need to set UpdateTime
        };

        using (var context = new CardDbContext())
        {
          context.Database.EnsureCreated();
            context.Cards.Add(testCard);
            context.SaveChanges();
        }

        Console.WriteLine("Card added to the database successfully");
    }
}