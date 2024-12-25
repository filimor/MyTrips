using AutoMapper;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Mappings;

public class DtoToDomainMappingProfile : Profile
{
    public DtoToDomainMappingProfile()
    {
        CreateMap<ClientDto, Client>().ReverseMap();
    }
}