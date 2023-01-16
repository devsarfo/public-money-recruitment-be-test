using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Responses;
using VacationRental.Core.Interfaces;
using VacationRental.Core.Models;

namespace VacationRental.Api.Controllers;

[Route("api/v1/rentals")]
[ApiController]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpGet]
    [Route("{rentalId:int}")]
    public async Task<RentalViewModel> Get(int rentalId)
    {
        var rental = await _rentalService.GetByIdAsync(rentalId) ?? throw new ApplicationException("Rental not found");
        return new RentalViewModel
        {
            Id = rental.Id,
            Units = rental.Units.Count,
            PreparationTimeInDays = rental.PreparationTimeInDays
        };
    }

    [HttpPost]
    public async Task<ResourceIdViewModel> Post(RentalBindingModel model)
    {
        var rental = await _rentalService.CreateAsync(new RentalBindingModel
        {
            Units = model.Units,
            PreparationTimeInDays = model.PreparationTimeInDays
        });
            
        return new ResourceIdViewModel { Id = rental.Id };
    }
        
    [HttpPut]
    [Route("{rentalId:int}")]
    public async Task<ResourceIdViewModel> Update(int rentalId, RentalBindingModel model)
    {
        if (model.Units <= 0) throw new ApplicationException("Rental must have at least one unit");
        if (model.PreparationTimeInDays < 0) throw new ApplicationException("Preparation Time In Days must be at least zero");
            
        var rental = await _rentalService.UpdateAsync(rentalId, model);
            
        return new ResourceIdViewModel { Id = rental.Id };
    }
}