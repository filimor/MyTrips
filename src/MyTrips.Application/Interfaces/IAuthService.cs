using MyTrips.Domain.Entities;

namespace MyTrips.Application.Interfaces;

public interface IAuthService
{
    string GetToken(LoginInfo loginInfo);
}