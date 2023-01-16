using System;
using Microsoft.EntityFrameworkCore;
using VacationRental.Core.Database;
using VacationRental.Core.Interfaces;
using VacationRental.Core.Services;

namespace VacationRental.Api.Tests.UnitTests;

public class BaseTest
{
    private readonly DatabaseContext _databaseContext;

    public BaseTest()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("VacationRentalDb")
            .Options;

        _databaseContext = new DatabaseContext(options);
    }
    
    protected T GetService<T>()
    {
        return (T)Activator.CreateInstance(typeof(T), _databaseContext);
    }
}