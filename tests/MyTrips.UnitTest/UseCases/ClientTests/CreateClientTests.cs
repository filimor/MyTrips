using System.Globalization;
using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentResults;
using Moq;
using MyTrips.Application.Dtos;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;

namespace MyTrips.UnitTest.UseCases.ClientTests;

public class CreateClientTests
{
    private readonly Mock<IClientsRepository> _clientsRepositoryMock = new();
    private readonly Client _fakeClient;
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly RequestClientDto _requestClientDto;
    private readonly ResponseClientDto _responseResponseClientDto;

    public CreateClientTests()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        _fakeClient = new Faker<Client>()
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        _responseResponseClientDto = new ResponseClientDto
        {
            Name = _fakeClient.Name,
            Email = _fakeClient.Email
        };

        _requestClientDto = new RequestClientDto
        {
            Name = _fakeClient.Name,
            Email = _fakeClient.Email
        };

        _mapperMock.Setup(m => m.Map<Client>(It.IsAny<RequestClientDto>()))
            .Returns((RequestClientDto requestClientDto) => new Client
            {
                Name = requestClientDto.Name,
                Email = requestClientDto.Email
            });

        _mapperMock.Setup(m => m.Map<ResponseClientDto>(It.IsAny<Client>()))
            .Returns((Client client) => new ResponseClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email
            });
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("layer", "Application")]
    public async Task GivenValidClient_WhenCreateClient_ThenItShouldReturnOkResultObjectWithTheDto()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync(_fakeClient.Id)).ReturnsAsync(_fakeClient);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        var testResult = Result.Ok(_responseResponseClientDto);

        // Act
        var clientResult = await clientsService.AddNewClientAsync(_requestClientDto);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    //[Fact]
    //[Trait("Category", "Unit")]
    //[Trait("layer", "Application")]
    //public async Task GivenClientWithId_WhenCreateClient_ThenItShouldIgnoreIdAndReturnOkResultObjectWithTheDto()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //[Trait("layer", "Application")]
    //public async Task GivenInvalidName_WhenCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //[Trait("layer", "Application")]
    //public async Task GivenInvalidEmail_WhenCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //[Trait("layer", "Application")]
    //public async Task GivenExistingEmail_WhenCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //[Trait("layer", "Application")]
    //public async Task GivenCreateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    //{
    //}
}