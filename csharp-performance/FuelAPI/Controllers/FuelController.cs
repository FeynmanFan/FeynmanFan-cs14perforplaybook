using Microsoft.AspNetCore.Mvc;

namespace FakeFuelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuelController : ControllerBase
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Gets the current fuel price for the given FBO airport code.
    /// Always takes approximately 10 seconds.
    /// </summary>
    /// <param name="airportCode">ICAO or IATA airport code (e.g., KDFW, DFW)</param>
    /// <returns>Fuel price in USD per gallon</returns>
    /// <response code="200">Returns the fuel price</response>
    [HttpGet("{airportCode}")]
    [ProducesResponseType(typeof(FuelPriceResponse), 200)]
    public async Task<ActionResult<FuelPriceResponse>> GetFuelForFBO(string airportCode)
    {
        // Simulate slow external call / processing / database / vendor API
        await Task.Delay(TimeSpan.FromSeconds(10));

        // Random price between 140.00 and 190.00
        double min = 140.00;
        double max = 190.00;
        double price = min + (_random.NextDouble() * (max - min));

        // Round to 2 decimal places (typical fuel price format)
        price = Math.Round(price, 2);

        var response = new FuelPriceResponse
        {
            AirportCode = airportCode.ToUpperInvariant(),
            FuelPriceUsdPerGallon = price,
            Timestamp = DateTime.UtcNow,
            Currency = "USD",
            Unit = "gallon",
            Note = "Simulated price - not real data"
        };

        return Ok(response);
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