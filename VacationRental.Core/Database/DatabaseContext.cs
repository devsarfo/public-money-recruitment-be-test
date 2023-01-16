using Microsoft.EntityFrameworkCore;
using VacationRental.Core.Entities;

namespace VacationRental.Core.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // in memory database used for simplicity, change to a real db for production applications
        options.UseInMemoryDatabase("VacationRentalDb");
    }
    
    public DbSet<Rental> Rental { get; set; }
    
    public DbSet<Unit> Unit { get; set; }
    
    public DbSet<Booking> Booking { get; set; }

}