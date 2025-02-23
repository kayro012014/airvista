using flight.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class FlightsController : Controller
{
    private readonly AppDbContext _context;

    public FlightsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var flights = _context.Flights
            .Include(f => f.Airline)
            .Include(f => f.FromAirport)
            .Include(f => f.ToAirport)
            .ToList() ?? new List<Flight>(); // ✅ Ensures flights is never null

        return View(flights);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlight([FromBody] Flight flight)
    {
        if (ModelState.IsValid)
        {
            var airline = await _context.Airlines.FindAsync(flight.AirlineId);
            var fromAirport = await _context.Airports.FindAsync(flight.FromAirportId);
            var toAirport = await _context.Airports.FindAsync(flight.ToAirportId);

            if (airline == null || fromAirport == null || toAirport == null)
            {
                return BadRequest("Invalid airline or airport selected.");
            }

            // Assign foreign key references
            flight.Airline = airline;
            flight.FromAirport = fromAirport;
            flight.ToAirport = toAirport;

            // Generate flight code
            flight.GenerateFlightCode(airline.Name);

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return Json(new
            {
                flightCode = flight.FlightCode,
                airlineName = airline.Name,
                fromAirport = fromAirport.Name,
                toAirport = toAirport.Name,
                departureDateTime = flight.DepartureDateTime.ToString("g"),
                arrivalDateTime = flight.EstimatedArrivalDateTime.ToString("g")
            });
        }

        return BadRequest("Invalid flight details.");
    }

}
