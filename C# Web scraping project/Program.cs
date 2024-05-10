using C__Web_scraping_project.Models;
using CSharp_Web_scraping_project.Models;
using PuppeteerSharp;
using System;
using System.Net.NetworkInformation;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine(
            "Welcome to your own personal One Piece Trading Card market tracker app! Please choose an option:");
        Console.WriteLine("1. Register a new account");
        Console.WriteLine("2. Login with an existing account");
        Console.Write("Enter your choice (1 or 2): ");

        string choice = Console.ReadLine();
        bool continueApp = true;

        switch (choice)
        {
            case "1":
                Register();
                Console.WriteLine("Please log in with your new account.");
                UserService.UserInfo loginResultAfterRegistration = Login();
                bool isAuthenticatedAfterRegistration = loginResultAfterRegistration.IsAuthenticated;
                if (isAuthenticatedAfterRegistration)
                {
                    while (continueApp)
                    {
                        continueApp = await UserMenu(loginResultAfterRegistration);
                    }
                }
                break;
            case "2":
                UserService.UserInfo loginResult = Login();
                bool isAuthenticated = loginResult.IsAuthenticated;
                int userID = loginResult.UserID;
                string username = loginResult.Username;

                if (isAuthenticated)
                {
                    while (continueApp)
                    {
                        continueApp = await UserMenu(loginResult);
                    }
                }
                break;
            default:
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                break;
        }
        Console.WriteLine("Thank you for using the One Piece Trading Card market tracker app!");
    }

    // Add a new method that greets the user and asks whether they want to get new card data or view the data they have already collected
    static async Task<bool> UserMenu(UserService.UserInfo loginResult)
    {
        Console.WriteLine("Welcome " + loginResult.Username + "!");
        Console.WriteLine();
        Console.WriteLine("Would you like to get new card data or view the data you have already collected? (new/view)");
        string choice = Console.ReadLine();
        Console.WriteLine();

        switch (choice)
        {
            case "new":
                List<string> cardNames = new List<string>();
                await CollectCardNames(cardNames);

                Console.WriteLine();
                Console.WriteLine("The application is now going to gather data for your card list.");

                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = "C:/Program Files/Google/Chrome/Application/chrome.exe"
                });

                var page = await browser.NewPageAsync();
                await page.GoToAsync("https://www.tcgplayer.com");

                await GatherCardData(cardNames, page, loginResult.UserID);
                break;
            case "view":
                Console.WriteLine("Here is the data you have collected so far:");
                Console.WriteLine();
                var cardTitles = GetUniqueCardTitlesByUserID(loginResult.UserID);
                Dictionary<int, string> cardNumberTitleMap = new Dictionary<int, string>();
                Console.WriteLine("Here are the cards you have saved to your account");
                Console.WriteLine();
                for (int i = 0; i < cardTitles.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {cardTitles[i]}");
                    cardNumberTitleMap[i + 1] = cardTitles[i];

                }
                Console.WriteLine();
                Console.WriteLine("Which card would you like to view your data for? (Enter the number or Enter 'all')");
                string userInput = Console.ReadLine();
                if (int.TryParse(userInput, out int cardNumber))
                {
                    // User entered a valid card number
                    if (cardNumberTitleMap.ContainsKey(cardNumber))
                    {
                        string selectedCardTitle = cardNumberTitleMap[cardNumber];
                        await GetCardDataByTitle(selectedCardTitle, loginResult.UserID);
                    }
                    else
                    {
                        Console.WriteLine("Invalid card number. Please try again.");
                    }
                }
                else if (userInput.ToLower() == "all")
                {
                    // User entered 'all'
                    await GetCardDataByTitle("all", loginResult.UserID);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }

                break;
            default:
                Console.WriteLine("Invalid choice. Please enter 'new' or 'view'.");
                break;
        }
        Console.WriteLine("Do you want to go back to the menu or exit? (menu/exit)");
        string userDecision = Console.ReadLine();
        if (userDecision.ToLower() == "menu")
        {
            return true; // User wants to return to the menu
        }
        else
        {
            return false; // User wants to exit
        }
    }
    // Gets unique card titles by user ID
    public static List<string> GetUniqueCardTitlesByUserID(int userID)
    {
        using (var context = new CardDbContext())
        {
            var uniqueTitles = context.tblCardDetails
                .Where(card => card.UserID == userID)
                .Select(card => card.Title)
                .Distinct()
                .ToList();
            return uniqueTitles;
        }
    }
    // Write a new method that gets card data based on the user's selection in the UserMenu method
    static async Task GetCardDataByTitle(string title, int userID)
    {
        using (var context = new CardDbContext())
        {
            IQueryable<Card> cardData;

            if (title.ToLower() == "all")
            {
                cardData = context.tblCardDetails
                    .Where(card => card.UserID == userID)
                    .OrderBy(card => card.Title);
            }
            else
            {
                cardData = context.tblCardDetails
                    .Where(card => card.Title == title && card.UserID == userID);
            }

            foreach (var card in cardData)
            {
                Console.WriteLine($"Title: {card.Title}");
                Console.WriteLine($"Rarity: {card.Rarity}");
                Console.WriteLine($"Quantity of Listings: {card.QuantityOfListings}");
                Console.WriteLine($"Market Price: {card.MarketPrice}");
                Console.WriteLine($"Time When Logged: {card.UpdateTime}");
                Console.WriteLine();
            }
        }
    }

    // Add a new method to handle the card data gathering
    static async Task GatherCardData(List<string> cardNames, IPage page, int userID)
    {
        foreach (var cardName in cardNames)
        {
            // Search for card
            await page.WaitForSelectorAsync("input[id=autocomplete-input]");
            await page.TypeAsync("input[id=autocomplete-input]", cardName);
            await page.Keyboard.PressAsync("Enter");
            await page.WaitForSelectorAsync("section[class=product-card__product]");
            await page.ClickAsync("section[class=product-card__product]");

            // Get card title
            await page.WaitForSelectorAsync("h1[class=product-details__name]");
            var title = await page.EvaluateExpressionAsync<string>("document.querySelector('h1[class=product-details__name]').innerText");

            // Get card rarity
            var rarityElement = await page.XPathAsync("//strong[contains(text(), 'Rarity:')]/following-sibling::span");
            var rarity = await rarityElement.FirstOrDefault()?.EvaluateFunctionAsync<string>($"el => el.textContent");

            // Get quantity of listings for card
            await page.WaitForSelectorAsync("span[class=view-all-listings__other-listings]");
            await Task.Delay(2000);
            var quantityOfListingsText = await page.EvaluateExpressionAsync<string>("document.querySelector('span[class=view-all-listings__other-listings]').innerText");
            if (quantityOfListingsText == "No Listings Available")
            {
                await page.GoToAsync("https://www.tcgplayer.com");
                continue;
            }
            var quantityOfListingsValue = int.Parse(quantityOfListingsText.Trim().Split(' ')[1]);

            // Get average market price for card
            await page.WaitForSelectorAsync("span[class=spotlight__price]");
            var marketPriceText = await page.EvaluateExpressionAsync<string>("document.querySelector('span[class=spotlight__price]').innerText");
            var marketPriceValue = decimal.Parse(marketPriceText.Replace("$", ""));

            Console.WriteLine($"{title} average market price is ${marketPriceValue} and there are {quantityOfListingsValue} listings available.");
            Console.WriteLine();
            Console.WriteLine("Would you like to log this card to your database? (yes/no)");
            Console.WriteLine();
            if (Console.ReadLine().ToLower() == "yes")
            {
                await LogCardToDatabase(title, rarity, quantityOfListingsValue, marketPriceValue, userID);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Card not logged.");
            }
            await page.GoToAsync("https://www.tcgplayer.com");
        }
    }

    // Method that loops to collect card names from user input
    static async Task CollectCardNames(List<string> cardNames)
    {
        Console.WriteLine("Please enter the card names you would like to track. Be sure to enter the card name exactly as it appears on tcgplayer.com. Type 'done' when you are finished:");
        Console.WriteLine();

        while (true)
        {
            Console.Write("Enter a card name or 'done' to finish: ");
            Console.WriteLine();
            string input = Console.ReadLine();

            if (input.ToLower() == "done")
            {
                Console.WriteLine("Are you sure you are done? (yes/no)");
                Console.WriteLine();
                string confirmation = Console.ReadLine();
                if (confirmation.ToLower() == "yes")
                {
                    Console.WriteLine("Here is your watch list:");
                    Console.WriteLine();
                    foreach (var card in cardNames)
                    {
                        Console.WriteLine(card);
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Please continue entering card names.");
                    Console.WriteLine();
                }
            }
            else
            {
                cardNames.Add(input);
                Console.WriteLine();
                Console.WriteLine($"{input} added to your watch list.");
                Console.WriteLine();
            }
        }
    }

    // Log card to database method
    static async Task LogCardToDatabase(string title, string rarity, int quantityOfListingsValue, decimal marketPriceValue, int userId)
{
        var CardToLog = new Card
        {
            Title = title,
            Rarity = rarity,
            QuantityOfListings = quantityOfListingsValue,
            MarketPrice = marketPriceValue,
            UserID = userId
        };

        using (var context = new CardDbContext())
        {
            context.tblCardDetails.Add(CardToLog);
            context.SaveChanges();
        }

        Console.WriteLine();
        Console.WriteLine($"{title} data successfully saved at {DateTime.Now}.");
    }



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
    static UserService.UserInfo Login()
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
            return new UserService.UserInfo() { IsAuthenticated = true, UserID = userService.GetUserInfo(username).UserID, Username = userService.GetUserInfo(username).Username};
        }
        else
        {
            Console.WriteLine("Login failed. Please check your username and password.");
            return new UserService.UserInfo() { IsAuthenticated = false, UserID = 0, Username = null };
        }
    }
}