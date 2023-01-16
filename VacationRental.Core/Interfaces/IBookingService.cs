using System.Linq.Expressions;
using VacationRental.Core.Entities;

namespace VacationRental.Core.Interfaces;

public interface IBookingService
{
    Task<Booking?> GetByIdAsync(int id);
    
    Task<List<Booking>> GetAllAsync(int? rentalId = null, Expression<Func<Booking, bool>>? filter = null);
    
    Task<Booking> CreateAsync(Rental rental, DateTime start, int nights);
}