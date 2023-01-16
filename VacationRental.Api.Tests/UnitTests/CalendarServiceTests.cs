using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VacationRental.Core.Entities;
using VacationRental.Core.Interfaces;
using VacationRental.Core.Models;
using VacationRental.Core.Services;
using Xunit;

namespace VacationRental.Api.Tests.UnitTests;

public class CalendarServiceTests : BaseTest
{
    private readonly RentalService _rentalService;
    private readonly BookingService _bookingService;
    private readonly CalendarService _calendarService;
    
    private CalendarService GetCalendarService()
    {
        return new CalendarService(_bookingService, _rentalService);
    }
    
    public CalendarServiceTests()
    {
        _bookingService = GetService<BookingService>();
        _rentalService = GetService<RentalService>();
        _calendarService = GetCalendarService();
    }

    [Fact]
    public async Task TestGetByCalendarAsync()
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, PreparationTimeInDays = 2
        });

        var booking = await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        
        var calendar = await _calendarService.GetByCalendarAsync(rental.Id, DateTime.Now.Date, 2);
        
        Assert.NotNull(calendar);
        Assert.Equal(2, calendar.Dates.Count);
        
        //Check Booking
        Assert.Single(calendar.Dates[0].Bookings);
        Assert.Empty(calendar.Dates[0].PreparationTimes);
        Assert.Equal(booking.Unit.Number, calendar.Dates[0].Bookings[0].Unit);

        //Check Preparation Dates
        Assert.Empty(calendar.Dates[1].Bookings);
        Assert.Single(calendar.Dates[1].PreparationTimes);
        Assert.Equal(booking.Unit.Number, calendar.Dates[1].PreparationTimes[0].Unit);

        
    }
    
    [Fact]
    public async Task TestGetByCalendarAsync_Throws_When_Rental_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _calendarService.GetByCalendarAsync(1000, DateTime.Now.Date, 1));
        Assert.Equal("Rental not found", exception.Message);
    }
}