using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace CatalogApp.Pages
{
    public class FuelModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly HybridCache _cache;

        private const string URL_TEMPLATE = "https://localhost:7106/api/fuel/";

        public FuelModel(HttpClient httpClient, HybridCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        [BindProperty(SupportsGet = true)]
        public string AirportCode { get; set; } = "KDFW";

        public FuelPriceResponse? Result { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public double ElapsedSeconds { get; set; }

        // ==============================================
        // Version 1: Direct API Call (No Cache)
        // ==============================================
        public async Task<IActionResult> OnPostDirect()
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var response = await _httpClient.GetAsync($"{URL_TEMPLATE}{AirportCode}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                Result = JsonSerializer.Deserialize<FuelPriceResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                StatusMessage = "Fetched directly from API (no cache)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }

            sw.Stop();
            ElapsedSeconds = Math.Round(sw.Elapsed.TotalSeconds, 2);

            return Page();
        }

        // ==============================================
        // Version 2: Redis Cache + API Fallback
        // ==============================================
        public async Task<IActionResult> OnPostCached()
        {
            var sw = Stopwatch.StartNew();
            var cacheKey = $"fuel:{AirportCode.ToUpperInvariant()}";

            try
            {
                // 1. Try to get from Redis cache
                Result = await _cache.GetOrCreateAsync(key: cacheKey, async ct =>
                {
                    StatusMessage = "Cache miss - fetching from API...";

                    var response = await _httpClient.GetAsync($"{URL_TEMPLATE}{AirportCode}", ct);

                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(ct);
                    var data = JsonSerializer.Deserialize<FuelPriceResponse>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    StatusMessage = "Fetched from API and stored in cache";

                    return data;
                }, 
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(5),
                    LocalCacheExpiration = TimeSpan.FromSeconds(30) // Local cache is much shorter to demonstrate cache hit/miss
                }
                );
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }

            sw.Stop();
            ElapsedSeconds = Math.Round(sw.Elapsed.TotalSeconds, 2);

            return Page();
        }
    }

    public class FuelPriceResponse
    {
        public string AirportCode { get; set; } = string.Empty;
        public double FuelPriceUsdPerGallon { get; set; }
        public DateTime Timestamp { get; set; }
        public string Currency { get; set; } = "USD";
        public string Unit { get; set; } = "gallon";
        public string? Note { get; set; }
    }
}