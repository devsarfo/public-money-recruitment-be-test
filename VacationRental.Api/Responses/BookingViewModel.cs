using System;

namespace VacationRental.Api.Responses;

public class BookingViewModel
{
    public int Id { get; set; }
        
    public int RentalId { get; init; }
        
    public DateTime Start { get; init; }
        
    public int Nights { get; init; }
}