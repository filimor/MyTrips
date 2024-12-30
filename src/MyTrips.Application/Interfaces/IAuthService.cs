using MyTrips.Domain.ValueObjects;

namespace MyTrips.Application.Interfaces;

public interface IAuthService
{
    string GetToken(LoginInfo loginInfo);
}