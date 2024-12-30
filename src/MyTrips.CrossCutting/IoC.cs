﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTrips.Application.Mappings;
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
        InjectRepositories(services, configuration);
        AddMapping(services);
        UseRepoDb();

        return services;
    }

    private static void UseRepoDb()
    {
        GlobalConfiguration.Setup().UseSqlServer();
        FluentMapper.Entity<Client>().Table("Clients");
    }

    private static void AddMapping(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DtoToDomainMappingProfile));
    }

    private static void InjectRepositories(IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<IClientsRepository>(_ => new ClientsRepository(configuration));
        services.AddScoped<IRepositoryBase, RepositoryBase<SqlConnection>>();
    }
}