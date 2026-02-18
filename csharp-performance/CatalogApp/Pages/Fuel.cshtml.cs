namespace CatalogApp.Pages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class FuelModel : PageModel
    {
        public const string API_CLIENT = "https://localhost:7106/";
        public const string API_CLIENT_NAME = "Long-lived API client";

        [BindProperty(SupportsGet = true)]
        public string AirportCode { get; set; } = "KDFW";

        public FuelPriceResponse? Result { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public bool IsLoading { get; set; } = false;

        // Initial GET – just render page; client JS will handle SignalR
        public void OnGet()
        {
            // Nothing special here; spinner is controlled on client side.
            IsLoading = false;
        }
    }

    public class FuelPriceResponse
    {
        public string AirportCode { get; set; } = string.Empty;
        public double FuelPriceUsdPerGallon { get; set; }
        public string Currency { get; set; } = "USD";
        public string Unit { get; set; } = "gallon";
        public string? Note { get; set; }
    }
}