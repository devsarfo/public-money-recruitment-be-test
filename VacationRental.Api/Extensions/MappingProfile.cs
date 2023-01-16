using AutoMapper;
using VacationRental.Api.Models;
using VacationRental.Api.Responses;
using VacationRental.Core.Entities;

namespace VacationRental.Api.Extensions;

public class MappingProfile : Profile
{

    public MappingProfile()
    {
        CreateMap<Calendar, CalendarViewModel>();
        CreateMap<CalendarDate, CalendarDateViewModel>();
        CreateMap<CalendarBooking, CalendarBookingViewModel>();
        CreateMap<CalendarPreparationTime, CalendarPreparationTimeViewModel>();
    }
}