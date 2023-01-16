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

public class BookingServiceTests : BaseTest
{
    private readonly RentalService _rentalService;
    private readonly BookingService _bookingService;
    
    public BookingServiceTests()
    {
        _bookingService = GetService<BookingService>();
        _rentalService = GetService<RentalService>();
    }

    [Fact]
    public async Task TestGetByIdAsync()
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, PreparationTimeInDays = 2
        });

        var expected = await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        var actual = await _bookingService.GetByIdAsync(expected.Id);

        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public async Task TestGetAllByIdAsync()
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, PreparationTimeInDays = 2
        });

        await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        
        var actual = await _bookingService.GetAllAsync(rental.Id);
        Assert.Equal(2, actual.Count);
    }
    
    
    [Fact]
    public async Task TestGetAllAsync_Filter()
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, PreparationTimeInDays = 2
        });

        var booking1 = await _bookingService.CreateAsync(rental, DateTime.Now.Date, 2);
        var booking2 = await _bookingService.CreateAsync(rental, DateTime.Now.AddDays(rental.PreparationTimeInDays + 1).Date, 2);
        var booking3 = await _bookingService.CreateAsync(rental, DateTime.Now.AddDays(rental.PreparationTimeInDays + 2).Date, 2);

        var filteredBookings = await _bookingService.GetAllAsync(rental.Id, b => b.Start == DateTime.Now.Date);
        Assert.Single(filteredBookings);
        Assert.Equal(booking1, filteredBookings.First());
    }
    
    [Fact]
    public async Task TestCreateAsync_Throws_When_Not_Available()
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 1, 
            PreparationTimeInDays = 2
        });
        
        await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _bookingService.CreateAsync(rental, DateTime.Now.Date, 1));
        
        Assert.Equal("Not available", exception.Message);
    }
}