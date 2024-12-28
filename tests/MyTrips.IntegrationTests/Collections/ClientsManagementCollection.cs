using MyTrips.IntegrationTests.Fixtures;

namespace MyTrips.IntegrationTests.Collections;

[CollectionDefinition("ClientsManagementIntegration", DisableParallelization = true)]
public class ClientsManagementCollection : ICollectionFixture<ClientsManagementFixture>
{
}