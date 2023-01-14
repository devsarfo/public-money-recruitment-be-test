using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Responses;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public CalendarController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            var result = new CalendarViewModel 
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>() 
            };
            
            var rental = _rentals[rentalId];
            
            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>()
                };

                var bookings = _bookings.Values.Where(booking => booking.RentalId == rentalId && booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date).ToList();
                if (bookings.Any())
                {
                    date.Bookings = bookings.Select(booking => new CalendarBookingViewModel
                    {
                        Id = booking.Id,
                        Unit = rental.Units
                    }).ToList();
                    
                    date.PreparationTimes = bookings.Select(booking => new CalendarPreparationTimeViewModel
                    {
                        Unit = rental.PreparationTimeInDays
                    }).ToList();
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}
