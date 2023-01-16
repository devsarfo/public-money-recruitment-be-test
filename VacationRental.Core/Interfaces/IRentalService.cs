using VacationRental.Core.Entities;
using VacationRental.Core.Models;

namespace VacationRental.Core.Interfaces;

public interface IRentalService
{
    Task<Rental?> GetByIdAsync(int id);
    
    Task<Rental> CreateAsync(RentalBindingModel data);
    
    Task<Rental> UpdateAsync(int rentalId, RentalBindingModel data);
}