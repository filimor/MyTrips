﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Services;

public class AuthService(IConfiguration configuration) : IAuthService
{
    private readonly List<LoginInfo> _logins = [new() { Username = "Admin", Password = "Password" }];

    public string GetToken(LoginInfo loginInfo)
    {
        var loginUser =
            _logins.SingleOrDefault(x => x.Username == loginInfo.Username && x.Password == loginInfo.Password);

        if (loginUser == null) return string.Empty;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, loginInfo.Username)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var userToken = tokenHandler.WriteToken(token);
        return userToken;
    }
}