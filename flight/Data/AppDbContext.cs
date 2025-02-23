using flight.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace flight.Data;
public class AppDbContext : IdentityDbContext<Users>
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Airport> Airports { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.Airline)
            .WithMany()
            .HasForeignKey(f => f.AirlineId);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.FromAirport)
            .WithMany()
            .HasForeignKey(f => f.FromAirportId);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.ToAirport)
            .WithMany()
            .HasForeignKey(f => f.ToAirportId);
    }
}
