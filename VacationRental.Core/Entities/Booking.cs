using System.ComponentModel.DataAnnotations.Schema;

namespace VacationRental.Core.Entities;

public class Booking

{
    public int Id { get; set; }
    
    [ForeignKey("RentalId")]
    public int RentalId { get; set; }
    
    [ForeignKey("UnitId")]
    public int UnitId { get; set; }
    public Unit Unit { get; set; }
    
    public DateTime Start { get; set; }
    
    public int Nights { get; set; }
    
}