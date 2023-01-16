namespace VacationRental.Core.Entities;

public class CalendarDate
{
    public DateTime Date { get; init; }
        
    public List<CalendarBooking> Bookings { get; set; }
        
    public List<CalendarPreparationTime> PreparationTimes { get; set; }
}