using VacationRental.Core.Entities;
using VacationRental.Core.Interfaces;
using Calendar = VacationRental.Core.Entities.Calendar;

namespace VacationRental.Core.Services;

public class CalendarService : ICalendarService
{
    private readonly IBookingService _bookingService;
    private readonly IRentalService _rentalService;
    
    public CalendarService(IBookingService bookingService, IRentalService rentalService)
    {
        _bookingService = bookingService;
        _rentalService = rentalService;
    }
    
    public async Task<Calendar> GetByCalendarAsync(int rentalId, DateTime start, int nights)
    {
        var rental = await _rentalService.GetByIdAsync(rentalId) ?? throw new ApplicationException("Rental not found");

        var calendar = new Calendar
        {
            RentalId = rental.Id,
            Dates = new List<CalendarDate>(),
        };
        
        var dates = Enumerable.Range(0, nights).Select(day => new CalendarDate
        {
            Date = start.Date.AddDays(day),
            Bookings = new List<CalendarBooking>(),
        });
        
        foreach (var date in dates)
        {
            var bookings = await _bookingService.GetAllAsync(rental.Id, booking => booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date);
            date.Bookings = bookings.Select(booking => new CalendarBooking
            {
                Id = booking.Id,
                Unit = booking.Unit.Number
            }).ToList();
            
            var preparationTimes = await _bookingService.GetAllAsync(rental.Id, booking => date.Date >= booking.Start.AddDays(booking.Nights) && date.Date <= booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays));
            date.PreparationTimes = preparationTimes.Select(preparationTime => new CalendarPreparationTime()
            {
                Unit = preparationTime.Unit.Number
            }).ToList();

            calendar.Dates.Add(date);
        }
        
        return calendar;
    }

    
}