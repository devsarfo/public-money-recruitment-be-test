using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Responses;
using VacationRental.Core.Interfaces;
using VacationRental.Core.Models;

namespace VacationRental.Api.Controllers;

[Route("api/v1/bookings")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly IBookingService _bookingService;

    public BookingsController(IRentalService rentalService, IBookingService bookingService)
    {
        _rentalService = rentalService;
        _bookingService = bookingService;
    }

    [HttpGet]
    [Route("{bookingId:int}")]
    public async Task<BookingViewModel> Get(int bookingId)
    {
        var booking = await _bookingService.GetByIdAsync(bookingId) ?? throw new ApplicationException("Booking not found");
        return new BookingViewModel
        {
            Id = booking.Id,
            RentalId = booking.RentalId,
            Start = booking.Start,
            Nights = booking.Nights
        };
    }

    [HttpPost]
    public async Task<ResourceIdViewModel> Post(BookingBindingModel model)
    {
        if (model.Nights <= 0) throw new ApplicationException("Nights must be positive");
            
        var rental = await _rentalService.GetByIdAsync(model.RentalId) ?? throw new ApplicationException("Rental not found");
        var booking = await _bookingService.CreateAsync(rental, model.Start, model.Nights);
            
        return new ResourceIdViewModel { Id = booking.Id };
    }
}