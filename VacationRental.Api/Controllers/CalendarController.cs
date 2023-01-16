using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Responses;
using VacationRental.Core.Entities;
using VacationRental.Core.Interfaces;

namespace VacationRental.Api.Controllers;

[Route("api/v1/calendar")]
[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly IMapper _mapper;

    public CalendarController(ICalendarService calendarService, IMapper mapper)
    {
        _calendarService = calendarService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<CalendarViewModel> Get(int rentalId, DateTime start, int nights)
    {
        if (nights < 0) throw new ApplicationException("Nights must be at least one");
        var calendar = await _calendarService.GetByCalendarAsync(rentalId, start, nights);

        return _mapper.Map<Calendar, CalendarViewModel>(calendar);
    }
}