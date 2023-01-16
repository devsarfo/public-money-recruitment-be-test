namespace VacationRental.Core.Entities;

public class Rental
{
    public int Id { get; set; }
    
    public List<Unit> Units { get; set; }
    
    public int PreparationTimeInDays { get; set; }
}