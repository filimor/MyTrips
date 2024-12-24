using MyTrips.Domain.Interfaces;

namespace MyTrips.UnitTest;

public class ClientsService(IClientRepository clientRepository)
{
    public async Task<object> GetClientsAsync()
    {
        return await clientRepository.GetAsync();
    }
}