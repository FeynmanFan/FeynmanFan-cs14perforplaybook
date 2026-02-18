using Microsoft.AspNetCore.SignalR;

namespace CatalogApp.Hubs
{
    public class FuelHub : Hub
    {
        public const string FuelUpdatedMethod = "FuelUpdated";
    }
}