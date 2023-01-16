using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VacationRental.Core.Database;
using VacationRental.Core.Entities;
using VacationRental.Core.Interfaces;

namespace VacationRental.Core.Services;

public class BookingService : IBookingService
{
    private readonly DatabaseContext _databaseContext;
    private DbSet<Booking> Bookings => _databaseContext.Booking;
    
    public BookingService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Booking?> GetByIdAsync(int id)
    {
        var booking = await Bookings.Where(r => r.Id == id).FirstOrDefaultAsync();
        return booking;
    }
    
    public Task<List<Booking>> GetAllAsync(int? rentalId = null, Expression<Func<Booking, bool>>? filter = null)
    {
        var bookings = Bookings.Include(b => b.Unit).AsQueryable();
 
        if (rentalId != null) 
            bookings = bookings.Where(b => b.RentalId == rentalId);
        
        if (filter != null) 
            bookings = bookings.Where(filter);
        
        return Task.FromResult(bookings.ToList());
    }

    public async Task<Booking> CreateAsync(Rental rental, DateTime start, int nights)
    {
        var bookings = await GetAllAsync(rental.Id, booking => 
            (booking.Start <= start.Date && booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays) > start.Date)
            || (booking.Start < start.AddDays(nights) && booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays) >= start.AddDays(nights))
            || (booking.Start > start && booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays) < start.AddDays(nights)));

        //Check Availability
        if (bookings.Count >= rental.Units.Count) throw new ApplicationException("Not available");
        
        var unit = rental.Units.FirstOrDefault(u => bookings.All(b => b.UnitId != u.Id));
        
        var booking = new Booking
        {
            RentalId = rental.Id,
            UnitId = unit!.Id,
            Nights = nights,
            Start = start
        };

        await Bookings.AddAsync(booking);
        await _databaseContext.SaveChangesAsync();

        return booking;
    }
}