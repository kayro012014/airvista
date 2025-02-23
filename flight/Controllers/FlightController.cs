
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
            .ToList();
        return View("Flights", flights);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlight(Flight flight)
    {
        if (ModelState.IsValid)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction("Flights"); // Reloads Flights.cshtml
        }
        return View("Flights", _context.Flights.ToList());
    }
}