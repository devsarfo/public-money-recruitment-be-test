namespace VacationRental.Core.Entities;

public class Calendar

{
    public int RentalId { get; set; }
        
    public List<CalendarDate> Dates { get; set; }
}