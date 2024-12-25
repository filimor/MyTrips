using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Infrastructure.Repositories;
using RepoDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClientsRepository, ClientsRepository>();
builder.Services.AddScoped<IClientsService, ClientsService>();

GlobalConfiguration.Setup().UseSqlServer();
FluentMapper.Entity<Client>().Table("Clients");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
///     Entrypoint
/// </summary>
public partial class Program
{
}