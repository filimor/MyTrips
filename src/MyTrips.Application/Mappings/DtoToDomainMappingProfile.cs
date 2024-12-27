using AutoMapper;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Mappings;

public class DtoToDomainMappingProfile : Profile
{
    public DtoToDomainMappingProfile()
    {
        CreateMap<CreateClientDto, Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Client, ResponseClientDto>();
        CreateMap<UpdateClientDto, Client>();
    }
}