using Microsoft.EntityFrameworkCore;
using VacationRental.Core.Database;
using VacationRental.Core.Entities;
using VacationRental.Core.Interfaces;
using VacationRental.Core.Models;

namespace VacationRental.Core.Services;

public class RentalService : IRentalService
{
    private readonly DatabaseContext _databaseContext;
    private DbSet<Rental> Rentals => _databaseContext.Rental;
    private DbSet<Unit> Units => _databaseContext.Unit;
    private DbSet<Booking> Bookings => _databaseContext.Booking;
    
    public RentalService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Rental?> GetByIdAsync(int id)
    {
        var rental = await Rentals.Where(r => r.Id == id).Include(r => r.Units).FirstOrDefaultAsync();
        return rental;
    }
    
    public async Task<Rental> CreateAsync(RentalBindingModel data)
    {
        var rental = new Rental
        {
            PreparationTimeInDays = data.PreparationTimeInDays
        };
        await Rentals.AddAsync(rental);
        
        var units = Enumerable.Range(1, data.Units)
            .Select(u => new Unit
            {
                RentalId = rental.Id,
                Number = u
            })
            .ToList();
        
        await Units.AddRangeAsync(units);
        
        await _databaseContext.SaveChangesAsync();
        
        return rental;
    }
    
    public async Task<Rental> UpdateAsync(int rentalId, RentalBindingModel data)
    {
        var rental = await GetByIdAsync(rentalId) ?? throw new ApplicationException("Rental not found");
        
        var bookings = Bookings.Include(b => b.Unit)
            .Where(b => b.RentalId == rentalId)
            .ToList();

        var date = DateTime.Now.Date;
        
        if (data.Units < rental.Units.Count)
        {
            for (var unit = data.Units + 1; unit <= rental.Units.Count; unit++)
            {
                var unitBookings = bookings.Where(b => 
                    b.RentalId == rentalId 
                    && b.Unit.Number == unit 
                    && (b.Start <= date && b.Start.AddDays(b.Nights) > date || b.Start > date)).ToList();
                
                if (unitBookings.Count > 0)
                {
                    throw new ApplicationException($"Unit {unit} cannot be removed, it has active bookings.");
                }
            }
        }
        
        if(data.PreparationTimeInDays > rental.PreparationTimeInDays)
        {
            if (bookings.Any(booking => HasOverlap(booking, bookings, data.PreparationTimeInDays)))
            {
                throw new ApplicationException("Cannot increase preparation time as it would cause overlapping bookings");
            }
        }
        
        if (data.Units > rental.Units.Count)
        {
            var newUnits = Enumerable.Range(rental.Units.Count + 1, data.Units - rental.Units.Count)
                .Select(i => new Unit { RentalId = rental.Id, Number = i })
                .ToList();
            Units.AddRange(newUnits);
        }
        else if (data.Units < rental.Units.Count)
        {
            var unitsToRemove = rental.Units.Where(u => u.Number > data.Units).ToList();
            Units.RemoveRange(unitsToRemove);
        }
        
        rental.PreparationTimeInDays = data.PreparationTimeInDays;

        await _databaseContext.SaveChangesAsync();
        return rental;
    }

    private static bool HasOverlap(Booking booking, IEnumerable<Booking> bookings, int newPreparationTime)
    {
        var newEnd = booking.Start.AddDays(booking.Nights).AddDays(newPreparationTime);
        var newStart = booking.Start.AddDays(-newPreparationTime);
        
        return bookings.Any(b => b.Id != booking.Id && b.UnitId != booking.UnitId && (newStart >= b.Start && newStart < b.Start.AddDays(b.Nights) || newEnd > b.Start && newEnd <= b.Start.AddDays(b.Nights)));
    }

}