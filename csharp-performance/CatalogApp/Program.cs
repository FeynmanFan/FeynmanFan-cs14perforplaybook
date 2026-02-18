using CatalogApp.Hubs;
using CatalogApp.Pages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;

namespace CatalogApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            
            builder.Services.AddHttpClient(FuelModel.API_CLIENT_NAME, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(30);
                client.BaseAddress = new Uri(FuelModel.API_CLIENT);
            });

            builder.Services.AddHybridCache();
            builder.Services.AddSignalR();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:2112";   // Change if your Redis is elsewhere
                options.InstanceName = "FuelCache_";
            });

            var app = builder.Build();
            app.MapPost("/api/fuel/check", async (FuelCheckRequest request,
                                      IHttpClientFactory httpFactory,
                                      HybridCache cache,
                                      IHubContext<FuelHub> hub) =>
            {
                var airportCode = request.AirportCode.ToUpperInvariant();
                var cacheKey = $"fuel:{airportCode}";

                // Fire-and-forget background work
                _ = Task.Run(async () =>
                {
                    string status = "connecting";
                    FuelPriceResponse? result = null;
                    string? error = null;

                    var client = httpFactory.CreateClient("FuelAPIClient");

                    try
                    {
                        result = await cache.GetOrCreateAsync(
                            key: cacheKey,
                            factory: async ct =>
                            {
                                var response = await client.GetAsync($"{FuelModel.API_CLIENT}api/fuel/{airportCode}", ct);
                                response.EnsureSuccessStatusCode();
                                var json = await response.Content.ReadAsStringAsync(ct);
                                return JsonSerializer.Deserialize<FuelPriceResponse>(json,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            },
                            options: new HybridCacheEntryOptions
                            {
                                Expiration = TimeSpan.FromMinutes(10),
                                LocalCacheExpiration = TimeSpan.FromMinutes(10)
                            });

                        status = "complete";
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        status = "error";
                    }
                    finally
                    {
                        await hub.Clients.All.SendAsync("ReceiveFuelUpdate", new
                        {
                            airportCode,
                            status,
                            result,
                            errorMessage = error
                        });
                    }
                });

                // Return immediately
                return Results.Accepted();
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.MapHub<FuelHub>("/fuelStatusHub");

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }

    public record FuelCheckRequest(string AirportCode);
}

