using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Responses
{
    public class CalendarViewModel
    {
        public int RentalId { get; set; }
        
        public List<CalendarDateViewModel> Dates { get; set; }
    }
}
