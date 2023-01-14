using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.Api.Models;
using VacationRental.Api.Responses;

namespace VacationRental.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDictionary<int, RentalViewModel>>(new Dictionary<int, RentalViewModel>());
        builder.Services.AddSingleton<IDictionary<int, BookingViewModel>>(new Dictionary<int, BookingViewModel>());

        return builder;
    }
}