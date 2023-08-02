using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

AssetDatabase database = new AssetDatabase();
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("----------Welcome to Assets Tracking System----------\n");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\nPlease follow the steps to create your Assets tracking\n");
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("Please enter to follow the steps | to quit - enter: 'Q' or quit");
Console.ResetColor();
while (true)
{
    string data = Console.ReadLine();
    if (data.ToLower().Trim() == "q")
    { break; }

    try
    {
        for (int i = 0; i < 2; i++) // How many assets must tracking like 6 for example if you have more assets so it can change from 2 here int i = 0; i < 2; i++ to 3 so it will be 3 computers 3 laptop and 3 phones totaly is 9 assets
        {
            StationaryComputers stationarycomputers = GetStationaryComputersDetailsFromUser(database);
            Laptop laptop = GetLaptopDetailsFromUser(database);
            MobilePhone mobilephone = GetMobilePhoneDetailsFromUser(database);
            database.AddAsset(mobilephone);
            database.AddAsset(stationarycomputers);
            database.AddAsset(laptop);
        }
        database.DisplaySortedAssets();
        static StationaryComputers GetStationaryComputersDetailsFromUser(AssetDatabase database)
        {
            Console.Write("Type: ");
            string StationaryComputersName = Console.ReadLine();
            Console.Write("Brand: ");
            string StationaryComputersBrand = Console.ReadLine();
            Console.Write("Model: ");
            string StationaryComputersModel = Console.ReadLine();
            Console.WriteLine("Office: ");
            string StationaryComputersOffice = Console.ReadLine();
            Console.Write("Purchase Date (YYYY/MM/DD): ");
            DateTime StationaryComputersPurchaseDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Price in USD: ");
            double StationaryComputersPrice = double.Parse(Console.ReadLine());
            string currency = database.GetCurrencyForOffice(StationaryComputersOffice);
            StationaryComputers stationarycomputers = new StationaryComputers(StationaryComputersName, StationaryComputersBrand, StationaryComputersModel, StationaryComputersOffice, StationaryComputersPurchaseDate, StationaryComputersPrice)
            {
                Currency = currency
            };
            return stationarycomputers;
        }
        static Laptop GetLaptopDetailsFromUser(AssetDatabase database)
        {
            Console.Write("Type: ");
            string laptopName = Console.ReadLine();
            Console.Write("Brand: ");
            string laptopBrand = Console.ReadLine();
            Console.Write("Model: ");
            string laptopModel = Console.ReadLine();
            Console.WriteLine("Office: ");
            string laptopOffice = Console.ReadLine();
            Console.Write("Purchase Date (YYYY/MM/DD): ");
            DateTime laptopPurchaseDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Price in USD: ");
            double laptopPrice = double.Parse(Console.ReadLine());
            string currency = database.GetCurrencyForOffice(laptopOffice);
            Laptop laptop = new Laptop(laptopName, laptopBrand, laptopModel, laptopOffice, laptopPurchaseDate, laptopPrice)
            {
                Currency = currency
            };
            return laptop;
        }
        static MobilePhone GetMobilePhoneDetailsFromUser(AssetDatabase database)
        {
            Console.Write("Type: ");
            string mobilephoneName = Console.ReadLine();
            Console.Write("Brand: ");
            string mobilephoneBrand = Console.ReadLine();
            Console.Write("Model: ");
            string mobilephoneModel = Console.ReadLine();
            Console.WriteLine("Office: ");
            string mobilephoneOffice = Console.ReadLine();
            Console.Write("Purchase Date (YYYY/MM/DD): ");
            DateTime mobilephonePurchaseDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Price in USD: ");
            double mobilephonePrice = double.Parse(Console.ReadLine());
            string currency = database.GetCurrencyForOffice(mobilephoneOffice);
            MobilePhone mobilephone = new MobilePhone(mobilephoneName, mobilephoneBrand, mobilephoneModel, mobilephoneOffice, mobilephonePurchaseDate, mobilephonePrice)
            {
                Currency = currency
            };
            return mobilephone;
        }
    }
    catch (FormatException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid date format. Please enter a Date (YYYY/MM/DD).");
        Console.ResetColor();
    }
}
class Asset
{
    internal object currency;
    public Asset(string name, string brand, string model, string office, DateTime purchaseDate, double price)
    {
        Name = name;
        Brand = brand;
        Model = model;
        Office = office;
        PurchaseDate = purchaseDate;
        Price = price;
    }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public DateTime PurchaseDate { get; set; }
    public double Price { get; set; }
    public string Office { get; set; }
    public string Currency { get; set; }
    public double Localpricetoday { get; set; }
}
class StationaryComputers : Asset
{ 
    public StationaryComputers(string name, string brand, string model, string office, DateTime purchaseDate, double price) : base(name, brand, model, office, purchaseDate, price)
    {
    }
}
class Laptop : Asset
{
    public Laptop(string name, string brand, string model, string office, DateTime purchaseDate, double price) : base(name, brand, model, office, purchaseDate, price)
    {
    }
}
class MobilePhone : Asset
{
    public MobilePhone(string name, string brand, string model, string office, DateTime purchaseDate, double price) : base(name, brand, model, office, purchaseDate, price)
    {
    }
}
class AssetDatabase
{
    private List<Asset> assets;
    private string currency;
    public AssetDatabase() { assets = new List<Asset>(); } // Default currency, can be changed based on the office later
    public void AddAsset(Asset asset) { assets.Add(asset); }
    public string GetCurrencyForOffice(string office)
    {
        if (office.ToLower() == "sweden")
        {
            return "SEK";
        }
        else if (office.ToLower() == "spain")
        {
            return "EUR";
        }
        else if (office.ToLower() == "usa")
        {
            return "USD";
        }
        else
        {
            return "USD"; // Set default currency if the office is not recognized
        }
    }
    private async Task<double> GetExchangeRate(string fromCurrency, string toCurrency) //Exchange rate
    {
        string apiUrl = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}/{toCurrency}"; // Free API rate exchange

        using (var httpClient = new HttpClient())
        {
            var json = await httpClient.GetAsync(apiUrl);
            json.EnsureSuccessStatusCode();
            var jsonBody = await json.Content.ReadAsStringAsync();
            var exchangeRateData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBody);
            double exchangeRate = Convert.ToDouble(exchangeRateData["conversion_rate"]);
            return exchangeRate;
        }
    }
    public async Task DisplaySortedAssets()
    {
        var sortedAsset = assets.OrderBy(a => a.Office).ThenBy(a => a.PurchaseDate).ToList();
        DateTime CurrentDate = DateTime.Now;
        Console.WriteLine("Type".PadRight(10) + "Brand".PadRight(10) + "Model".PadRight(10) + "Office".PadRight(10) + "Purchase Date".PadRight(15) + "Price in USD".PadRight(15) + "Currency".PadRight(10) + "Local Price Today".PadRight(10));
        Console.WriteLine("----".PadRight(10) + "-----".PadRight(10) + "-----".PadRight(10) + "------".PadRight(10) + "-------------".PadRight(15) + "------------".PadRight(15) + "--------".PadRight(10) + "-----------------".PadRight(10));
        foreach (Asset a in sortedAsset)
        {
            Console.Write(a.Name.PadRight(10) + a.Brand.PadRight(10) + a.Model.PadRight(10) + a.Office.PadRight(10) + a.PurchaseDate.ToShortDateString().PadRight(15) + a.Price.ToString().PadRight(15) + a.Currency.PadRight(10));
            if (IsEndOfLifeApproaching(a.PurchaseDate, CurrentDate))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (IsEndOfLifeApproaching6(a.PurchaseDate, CurrentDate))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            try
            {
                double exchangeRate = await GetExchangeRate("USD", a.Currency);
                double localPrice = a.Price * exchangeRate;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(localPrice.ToString("0.00").PadRight(10));
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("N/A".PadRight(10));
            }
            Console.WriteLine();
        }
    }
    private bool IsEndOfLifeApproaching(DateTime PurchaseDate, DateTime CurrentDate)
    {
        DateTime endOfLife = PurchaseDate.AddYears(3);
        DateTime threeMonthsFromNow = CurrentDate.AddMonths(3);
        return endOfLife <= threeMonthsFromNow;
    }
    private bool IsEndOfLifeApproaching6(DateTime PurchaseDate, DateTime CurrentDate)
    {
        DateTime endOfLife = PurchaseDate.AddYears(3);
        DateTime sixMonthsFromNow = CurrentDate.AddMonths(6);
        return endOfLife <= sixMonthsFromNow;
    }

}

