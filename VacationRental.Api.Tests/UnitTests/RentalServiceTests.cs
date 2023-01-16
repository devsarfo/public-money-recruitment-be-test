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

public class RentalServiceTests : BaseTest
{
    private readonly RentalService _rentalService;
    private readonly BookingService _bookingService;
    
    public RentalServiceTests()
    {
        _bookingService = GetService<BookingService>();
        _rentalService = GetService<RentalService>();
    }

    [Fact]
    public async Task TestGetByIdAsync()
    {
        var expected = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, PreparationTimeInDays = 2
        });

        var actual = await _rentalService.GetByIdAsync(expected.Id);

        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public async Task TestUpdateAsync()
    {
        var expected = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, PreparationTimeInDays = 2
        });

        var actual = await _rentalService.UpdateAsync(1, new RentalBindingModel
        {
            PreparationTimeInDays = 3, Units = 3
        });

        Assert.Equal(3, actual.PreparationTimeInDays);
        Assert.Equal(3, actual.Units.Count);
    }
    
    [Fact]
    public async Task TestUpdateAsync_Throws_When_Removing_Unit_With_Active_Booking()
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, 
            PreparationTimeInDays = 2
        });
        
        await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        
        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _rentalService.UpdateAsync(rental.Id, new RentalBindingModel
        {
            PreparationTimeInDays = 2, Units = 1
        }));
        
        Assert.Equal("Unit 2 cannot be removed, it has active bookings.", exception.Message);
    }

    [Fact]
    public async Task TestUpdateAsync_Throws_When_Increasing_Preparation_Time_Causes_Overlap()
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = 2, PreparationTimeInDays = 2
        });
        
        await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        await _bookingService.CreateAsync(rental, DateTime.Now.Date, 1);
        
        await _bookingService.CreateAsync(rental, DateTime.Now.Date.AddDays(rental.PreparationTimeInDays + 1), 1);

        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _rentalService.UpdateAsync(rental.Id, new RentalBindingModel
        {
            PreparationTimeInDays = 3, 
            Units = 2
        }));
        
        Assert.Equal("Cannot increase preparation time as it would cause overlapping bookings", exception.Message);
    }
}