using CSharp_Web_scraping_project.Models;
using PuppeteerSharp;
using System;
using System.Net.NetworkInformation;

class Program
{
    static async Task Main(string[] args)
    {
        var cardNames = new List<string>
        {
            "Gecko Moria (086) - Wings of the Captain (op06)",
            "Hody Jones (035) - Wings of the Captain (OP06)",
            "Vinsmoke Reiju (069) - Wings of the Captain (OP06)"
        };

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = "C:/Program Files/Google/Chrome/Application/chrome.exe"
        });

        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://www.tcgplayer.com");

        foreach (var cardName in cardNames)
        {
            await page.WaitForSelectorAsync("input[id=autocomplete-input]");
            await page.TypeAsync("input[id=autocomplete-input]", cardName);
            await page.Keyboard.PressAsync("Enter");
            await page.WaitForSelectorAsync("section[class=product-card__product]");
            await page.ClickAsync("section[class=product-card__product]");

            await page.WaitForSelectorAsync("h1[class=product-details__name]");
            var title = await page.EvaluateExpressionAsync<string>("document.querySelector('h1[class=product-details__name]').innerText");

            var rarityElement = await page.XPathAsync("//strong[contains(text(), 'Rarity:')]/following-sibling::span");
            var rarity = await rarityElement.FirstOrDefault()?.EvaluateFunctionAsync<string>($"el => el.textContent");

            await page.WaitForSelectorAsync("span[class=view-all-listings__other-listings]");
            
            await Task.Delay(2000);
            var quantityOfListingsText = await page.EvaluateExpressionAsync<string>("document.querySelector('span[class=view-all-listings__other-listings]').innerText");

            if (quantityOfListingsText == "No Listings Available")
            {
                await page.GoToAsync("https://www.tcgplayer.com");
                continue;
            }

            var quantityOfListingsValue = int.Parse(quantityOfListingsText.Trim().Split(' ')[1]);

            await page.WaitForSelectorAsync("span[class=spotlight__price]");
            var marketPriceText = await page.EvaluateExpressionAsync<string>("document.querySelector('span[class=spotlight__price]').innerText");
            var marketPriceValue = decimal.Parse(marketPriceText.Replace("$", ""));

            var CardToLog = new Card
            {
                Title = title,
                Rarity = rarity,
                QuantityOfListings = quantityOfListingsValue,
                MarketPrice = marketPriceValue,
            };

            using (var context = new CardDbContext())
            {
                context.tblCardDetails.Add(CardToLog);
                context.SaveChanges();
            }

            await page.GoToAsync("https://www.tcgplayer.com");
        }

        await browser.CloseAsync();
    }
}
