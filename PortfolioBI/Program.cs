using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;

namespace SecurityPriceAnalysis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string apiKey = "M9RGIRL00S3OU4BZ"; 

            var dailyData = await FetchStockData("GLW", apiKey);

            if (dailyData == null || !dailyData.Any())
            {
                Console.WriteLine("Failed to fetch stock data or no data available.");
                return;
            }

            // Calculate min, max, and average closing prices
            decimal minPrice = dailyData.Min(d => d.Close);
            decimal maxPrice = dailyData.Max(d => d.Close);
            decimal averagePrice = dailyData.Average(d => d.Close);

            // Identify the day with the largest percentage increase
            var largestIncrease = dailyData.OrderByDescending(d => (d.Close - d.Open) / d.Open).First();

            // Calculate the return on investment (ROI)
            decimal initialInvestment = 1000; // Initial investment amount
            decimal finalInvestment = initialInvestment * (largestIncrease.Close / largestIncrease.Open);
            decimal roi = (finalInvestment - initialInvestment) / initialInvestment;

            // Further analysis and visualization
            Console.WriteLine($"Min Price: {minPrice}");
            Console.WriteLine($"Max Price: {maxPrice}");
            Console.WriteLine($"Average Price: {averagePrice}");
            Console.WriteLine($"Largest Increase: {largestIncrease.Date}, {largestIncrease.Close}, {largestIncrease.Open}");
            Console.WriteLine($"ROI: {roi:P2}");
        }

        private static async Task<List<DailyData>> FetchStockData(string symbol, string apiKey)
        {
            var client = new RestClient($"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}");
            var request = new RestRequest();
            var response = await client.GetAsync(request);

            if (response == null || response.Content == null)
            {
                Console.WriteLine("Failed to retrieve data from API.");
                return null;
            }

            Console.WriteLine("API Response Content: " + response.Content);

            var jsonResponse = JObject.Parse(response.Content);
            var timeSeriesData = (JObject)jsonResponse["Time Series (Daily)"];

            if (timeSeriesData == null)
            {
                Console.WriteLine("Failed to parse JSON response. JSON content:");
                Console.WriteLine(response.Content);
                return null;
            }

            var dailyData = new List<DailyData>();
            foreach (var date in timeSeriesData)
            {
                var dailyInfo = (JObject)date.Value;
                dailyData.Add(new DailyData
                {
                    Date = DateTime.ParseExact(date.Key, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Open = (decimal)dailyInfo["1. open"],
                    High = (decimal)dailyInfo["2. high"],
                    Low = (decimal)dailyInfo["3. low"],
                    Close = (decimal)dailyInfo["4. close"],
                    Volume = (long)dailyInfo["5. volume"]
                });
            }

            return dailyData;
        }

    }
}
