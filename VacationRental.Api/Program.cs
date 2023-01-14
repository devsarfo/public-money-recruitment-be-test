using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using VacationRental.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(opt => opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Vacation Rental", Version = "v1" }));

builder.AddApiServices();

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Vacation Rental v1"));

app.Run();
