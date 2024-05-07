using C__Web_scraping_project.Models;
using CSharp_Web_scraping_project.Models;
using PuppeteerSharp;
using System;
using System.Net.NetworkInformation;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to your own personal One Piece Trading Card market tracker app! Please choose an option:");
        Console.WriteLine("1. Register a new account");
        Console.WriteLine("2. Login with an existing account");
        Console.Write("Enter your choice (1 or 2): ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Register();
                break;
            case "2":
                Login();
                break;
            default:
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                break;
        }


        //List<string> cardNames = new List<string>();
        //await CollectCardNames(cardNames);

        //Console.WriteLine();
        //Console.WriteLine("The application is now going to gather data for your card list.");

        //var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        //{
        //    Headless = true,
        //    ExecutablePath = "C:/Program Files/Google/Chrome/Application/chrome.exe"
        //});

        //var page = await browser.NewPageAsync();
        //await page.GoToAsync("https://www.tcgplayer.com");

        //foreach (var cardName in cardNames)
        //{
        //    // Search for card
        //    await page.WaitForSelectorAsync("input[id=autocomplete-input]");
        //    await page.TypeAsync("input[id=autocomplete-input]", cardName);
        //    await page.Keyboard.PressAsync("Enter");
        //    await page.WaitForSelectorAsync("section[class=product-card__product]");
        //    await page.ClickAsync("section[class=product-card__product]");

        //    // Get card title
        //    await page.WaitForSelectorAsync("h1[class=product-details__name]");
        //    var title = await page.EvaluateExpressionAsync<string>("document.querySelector('h1[class=product-details__name]').innerText");

        //    // Get card rarity
        //    var rarityElement = await page.XPathAsync("//strong[contains(text(), 'Rarity:')]/following-sibling::span");
        //    var rarity = await rarityElement.FirstOrDefault()?.EvaluateFunctionAsync<string>($"el => el.textContent");

        //    // Get quantity of listings for card
        //    await page.WaitForSelectorAsync("span[class=view-all-listings__other-listings]");
        //    await Task.Delay(2000);
        //    var quantityOfListingsText = await page.EvaluateExpressionAsync<string>("document.querySelector('span[class=view-all-listings__other-listings]').innerText");
        //    if (quantityOfListingsText == "No Listings Available")
        //    {
        //        await page.GoToAsync("https://www.tcgplayer.com");
        //        continue;
        //    }
        //    var quantityOfListingsValue = int.Parse(quantityOfListingsText.Trim().Split(' ')[1]);

        //    // Get average market price for card
        //    await page.WaitForSelectorAsync("span[class=spotlight__price]");
        //    var marketPriceText = await page.EvaluateExpressionAsync<string>("document.querySelector('span[class=spotlight__price]').innerText");
        //    var marketPriceValue = decimal.Parse(marketPriceText.Replace("$", ""));

        //    Console.WriteLine($"{title} average market price is ${marketPriceValue} and there are {quantityOfListingsValue} listings available.");
        //    Console.WriteLine();
        //    Console.WriteLine("Would you like to log this card to your database? (yes/no)");
        //    Console.WriteLine();
        //    if (Console.ReadLine().ToLower() == "yes")
        //    {
        //        await LogCardToDatabase(title, rarity, quantityOfListingsValue, marketPriceValue);
        //    }
        //    else
        //    {
        //        Console.WriteLine();
        //        Console.WriteLine("Card not logged.");
        //    }
        //    await page.GoToAsync("https://www.tcgplayer.com");
        //}

        //// Method that loops to collect card names from user input
        //static async Task CollectCardNames(List<string> cardNames)
        //{
        //    Console.WriteLine("Please enter the card names you would like to track. Be sure to enter the card name exactly as it appears on tcgplayer.com. Type 'done' when you are finished:");
        //    Console.WriteLine();

        //    while (true)
        //    {
        //        Console.Write("Enter a card name or 'done' to finish: ");
        //        Console.WriteLine();
        //        string input = Console.ReadLine();

        //        if (input.ToLower() == "done")
        //        {
        //            Console.WriteLine("Are you sure you are done? (yes/no)");
        //            Console.WriteLine();
        //            string confirmation = Console.ReadLine();
        //            if (confirmation.ToLower() == "yes")
        //            {
        //                Console.WriteLine("Here is your watch list:");
        //                Console.WriteLine();
        //                foreach (var card in cardNames)
        //                {
        //                    Console.WriteLine(card);
        //                }
        //                break;
        //            }
        //            else
        //            {
        //                Console.WriteLine("Please continue entering card names.");
        //                Console.WriteLine();
        //            }
        //        }
        //        else
        //        {
        //            cardNames.Add(input);
        //            Console.WriteLine();
        //            Console.WriteLine($"{input} added to your watch list.");
        //            Console.WriteLine();
        //        }
        //    }
        //}

        //// Log card to database method
        //static async Task LogCardToDatabase(string title, string rarity, int quantityOfListingsValue, decimal marketPriceValue)
        //{
        //    var CardToLog = new Card
        //    {
        //        Title = title,
        //        Rarity = rarity,
        //        QuantityOfListings = quantityOfListingsValue,
        //        MarketPrice = marketPriceValue,
        //    };

        //    using (var context = new CardDbContext())
        //    {
        //        context.tblCardDetails.Add(CardToLog);
        //        context.SaveChanges();
        //    }

        //    Console.WriteLine();
        //    Console.WriteLine($"{title} data successfully saved at {DateTime.Now}.");
        //}
        //Console.WriteLine();
        //Console.WriteLine("Thank you for using the One Piece Trading Card market tracker app!");
        //Console.WriteLine();
        //Console.WriteLine("Press any key to exit.");

        //await browser.CloseAsync();

        static void Register()
        {
            Console.WriteLine("To register a new account, please create a new username and password.");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            UserService userService = new UserService();
            bool success = userService.RegisterUser(username, password);

            if (success)
            {
                Console.WriteLine("Registration successful!");
            }
            else
            {
                Console.WriteLine("Registration failed. Please try again.");
            }
        }


        static void Login()
        {
            Console.WriteLine("To login, please enter your username and password.");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            UserService userService = new UserService();
            bool isAuthenticated = userService.AuthenticateUser(username, password);

            if (isAuthenticated)
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Login failed. Please check your username and password.");
            }
        }
    }
}
