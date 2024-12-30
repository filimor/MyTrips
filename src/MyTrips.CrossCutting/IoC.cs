using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTrips.Application.Mappings;
using MyTrips.CrossCutting.Handlers;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Infrastructure.Interfaces;
using MyTrips.Infrastructure.Logging;
using MyTrips.Infrastructure.Repositories;
using RepoDb;

namespace MyTrips.CrossCutting;

public static class IoC
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        InjectRepositories(services, configuration);
        AddMapping(services);
        UseRepoDb();

        return services;
    }

    private static void UseRepoDb()
    {
        GlobalConfiguration.Setup().UseSqlServer();

        FluentMapper.Entity<Client>().Table("Clients");
        FluentMapper.Entity<Trip>().Table("Trips");
        FluentMapper.Entity<Flight>().Table("Flights");
        FluentMapper.Entity<Hotel>().Table("Hotels");
        FluentMapper.Entity<Destination>().Table("Destinations");

        PropertyHandlerMapper.Add<DateOnly, DateOnlyPropertyHandler>();
        PropertyHandlerMapper.Add<DateOnly?, NullableDateOnlyPropertyHandler>();
    }

    private static void AddMapping(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DtoToDomainMappingProfile));
    }

    private static void InjectRepositories(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IClientsRepository, ClientsRepository>();
        services.AddScoped<ITripsRepository, TripsRepository>();

        services.AddSingleton<IMyTripsTrace, MyTripsTrace>();
    }
}