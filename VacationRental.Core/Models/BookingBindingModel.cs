namespace VacationRental.Core.Models;

public class BookingBindingModel
{
    public int RentalId { get; init; }

    public DateTime Start
    {
        get => _startIgnoreTime;
        init => _startIgnoreTime = value.Date;
    }

    private readonly DateTime _startIgnoreTime;
        
    public int Nights { get; init; }
}