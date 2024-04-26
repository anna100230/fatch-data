using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinnHub
{
    public class StockData
    {
        public int Count { get; set; }
        public List<StockLine> Result { get; set; }
    }

    public class StockLine
    {
        public string Description { get; set; }
        public string Symbol { get; set; }
        public string DisplaySymbol { get; set; }
        public string Type { get; set; }
    }

    public class StockService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly HttpClient _client;

        public StockService(string apiKey)
        {
            _apiKey = apiKey;
            _baseUrl = "";   // your website from where you want to fatch data
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<StockData> GetStockDataAsync(string symbol)
        {
            string url = $"{_baseUrl}{symbol}&token={_apiKey}";
            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<StockData>(json);
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string apiKey = "cokl9ppr01qo8vdu40ngcokl9ppr01qo8vdu40o0";
            string symbol;

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a stock symbol:");
                symbol = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(symbol))
                {
                    Console.WriteLine("Invalid stock symbol.");
                    return;
                }
            }
            else
            {
                symbol = args[0];
            }

            var stockService = new StockService(apiKey);
            try
            {
                StockData data = await stockService.GetStockDataAsync(symbol);
                Console.WriteLine($"Found {data.Count} results for '{symbol}':");
                if (data.Count != 0) 
                {
                    foreach (StockLine stock in data.Result)
                    {
                        Console.WriteLine($"- {stock.Symbol} ({stock.Description})");
                    }
                }
                
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

