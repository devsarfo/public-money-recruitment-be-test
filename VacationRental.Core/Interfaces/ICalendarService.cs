
using VacationRental.Core.Entities;

namespace VacationRental.Core.Interfaces;

public interface ICalendarService
{
    Task<Calendar> GetByCalendarAsync(int rentalId, DateTime start, int nights);
}