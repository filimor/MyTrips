using AutoMapper;
using MyTrips.Application.Dtos;
using MyTrips.Application.Mappings.Converters;
using MyTrips.Domain.Entities;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Application.Mappings;

public class DtoToDomainMappingProfile : Profile
{
    public DtoToDomainMappingProfile()
    {
        CreateMap<CreateClientDto, Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Client, ResponseClientDto>();
        CreateMap<UpdateClientDto, Client>();
        CreateMap<UpdateClientDto, ResponseClientDto>();

        CreateMap(typeof(PagedList<>), typeof(PagedList<>))
            .ConvertUsing(typeof(PagedListTypeConverter<,>));

        CreateMap<CreateTripDto, Trip>();
        CreateMap<Trip, ResponseTripDto>();
        CreateMap<Trip, ShortResponseTripDto>();
    }
}