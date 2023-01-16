using System.ComponentModel.DataAnnotations.Schema;

namespace VacationRental.Core.Entities;

public class Unit
{
    public int Id { get; set; }
    
    [ForeignKey("RentalId")]
    public int RentalId { get; set; }
    
    public int Number { get; set; }
    
    
}