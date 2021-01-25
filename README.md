<h1 align="center">Exchange API</h1>

# List of Projects

* [Exchange.API Backend](https://github.com/alexis-dotnet/ExchangeAPI/tree/main/Exchange.API) API for currency exchanging usin .NET 5
* [Exchange Client Frontend](https://github.com/alexis-dotnet/ExchangeAPI/tree/main/exchange-client) Angular Client for Exchange.API.

# Getting Started

## Backend

[Exchange.API Backend](https://github.com/alexis-dotnet/ExchangeAPI/tree/main/Exchange.API) contains all core components provided by NuGet packages, so you don't need to include external references to it. Also, the launch settings have been included in this repository to allow the Angular client to run after clonning it without any changes.

You need to have the following prerequisites to be able to compile and run it:

* SqlServer LocalDb
* .NET 5

This project is using SqlServer LocalDb as **(LocalDb)\\msqllocaldb**, and Entity Framework Core is being used to access the databse. SQL Scripts to create the database have not been included because the project scans the SqlServer LocalDb instance at runtime to look for the database, and if it doesn't exist, the code will create it based on the entities and mappings included in DbInitializer.cs. Also, several testing clients will be created on this step:

```
    public static class DbInitializer
    {
        public static void Initialize(MainContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
                return;

            var users = new User[]
            {
                new User { Name = "John Connor"},   // UserId = 1
                new User { Name = "Elon Musk"},     // UserId = 2
                new User { Name = "Juana Azurduy"}, // UserId = 3
                new User { Name = "Woody Allen"},   // UserId = 4
                new User { Name = "Isabel Allende"} // UserId = 5
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
```

The structure of the tables in the database is according to the enitites defined in the project. Entity Framework Core will take care of creating the relationships defined in these classes:

```
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public List<Purchase> Purchases { get; set; } = new();
    }

    public class Purchase
    {
        public int PurchaseId { get; set; }
        public decimal OriginAmount { get; set; }
        public string OriginCurrency { get; set; } = "ARS";
        public decimal TargetAmount { get; set; }
        public string TargetCurrency { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now.ToUniversalTime();

        public int UserId { get; set; }
        public User User { get; set; }
    }

```

Log files will be stored in the folder Logs, which will be created in the same location where the  DLL files are stored (eventually, this folder will be **..\Exchange.API\Exchange.API\bin\Debug\net5.0\Logs**).

You can change the limits for the current supported currencies by modifying those values in appsettings.json:


```
...
  "UsdExchangeUrl": "https://www.bancoprovincia.com.ar/Principal/Dolar",
  "BrlExchangeUrl": "https://www.bancoprovincia.com.ar/Principal/Dolar",
  "SupportedCurrencies": [
    {
      "Currency": "USD",
      "Limit": 200
    },
    {
      "Currency": "BRL",
      "Limit": 300
    }
  ]
...
```

However, even when you can change the limits of the currencies, you can't add other different currencies or replace the existing ones with others. The project will be able to add a new currency by adding a new "retriever" class in the Exchange.API.Services.Rate folder, following a structure similar to UsdRateRetriever:

```
    public class UsdRateRetriever : BaseRateRetriever, IRateRetriever
    {
        public UsdRateRetriever(ApiOptions settings, IHttpCallService httpService) : base(settings, httpService)  { }

        public async Task<RateResponseDto> GetRateAsync()
        {
            // TODO: implement the code to call the endpoint to request the information about the currency from its respective endpoint
            return ParseUsdRate(response);
        }

        private RateResponseDto ParseUsdRate(string response)
        {
			// TODO: implement the code to parse the information coming from the endpoint
            return rateResponse;
        }

    }
```

### About passing UserId as a parameter when calling the endpoints

Using the UserId as part of the request body is not so practical, because this is easy to automate. A better solution for this is to create a token or session ID related to the UserID which will be much harder to automate because this will not depend on static values, but on the session assigned to the user when he/she has been logged into the system.

## Frontend

[Exchange Client Frontend](https://github.com/alexis-dotnet/ExchangeAPI/tree/main/exchange-client) contains all core components provided by NPM packages, so you don't need to include external references to it. Also, the environment.ts file have been included in this repository to allow the Angular client to call the backend API after clonning this repository without any changes.

You need to have the following prerequisites for this project to be able to run:

* npm 6.* or newer

