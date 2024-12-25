using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Mappings;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Infrastructure.Repositories;
using RepoDb;

namespace MyTrips.CrossCutting;

public static class IoC
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IClientsRepository>(_ => new ClientsRepository(configuration));
        services.AddScoped<IClientsService, ClientsService>();
        services.AddAutoMapper(typeof(DtoToDomainMappingProfile));
        GlobalConfiguration.Setup().UseSqlServer();
        FluentMapper.Entity<Client>().Table("Clients");

        return services;
    }
}