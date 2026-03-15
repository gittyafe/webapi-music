// using System;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;
// using MusicWebapi.Api.Models;
// using MusicWebapi.Application.Interfaces;

// namespace MusicWebapi.Application.Services;

// public static class TokenService
// {
//     private static SymmetricSecurityKey key; 
//     // Use local server URL for issuer during development
//     private static string issuer = "https://localhost:7159";
//     private static string audience = "your-api-client";
//     static TokenService()
//     {
//         var config = new ConfigurationBuilder()
//             .SetBasePath(AppContext.BaseDirectory)
//             .AddJsonFile("appsettings.json", optional: true)
//             .AddJsonFile("appsettings.Development.json", optional: true)
//             .Build();
//         var keyString = config["Jwt:Key"];
//         if (string.IsNullOrEmpty(keyString) || keyString.Length < 32)
//             throw new Exception("JWT key is missing or too short (must be at least 32 characters) in appsettings.json or appsettings.Development.json");
//         key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
//         var issuerString = config["Jwt:Issuer"];
//         if (!string.IsNullOrEmpty(issuerString))
//             issuer = issuerString;
//         var audienceString = config["Jwt:Audience"];
//         if (!string.IsNullOrEmpty(audienceString))
//             audience = audienceString;
//     }

//     public static SecurityToken GetToken(List<Claim> claims) =>
//         new JwtSecurityToken(
//             issuer,
//             audience,
//             claims,
//             expires: DateTime.Now.AddDays(30.0),
//             signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
//         );

//     public static TokenValidationParameters GetTokenValidationParameters() =>
//         new TokenValidationParameters
//         {
//             ValidIssuer = issuer,
//             ValidAudience = audience,
//             IssuerSigningKey = key,
//             ClockSkew = TimeSpan.Zero // remove delay of token when expire
//         };

//     public static string WriteToken(SecurityToken token) =>
//         new JwtSecurityTokenHandler().WriteToken(token);
// }

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace MusicWebapi.Application.Services
{
    public static class TokenService
    {
        private static SymmetricSecurityKey key;
        private static string issuer;
        private static string audience;

        static TokenService()
        {
            // טוען את appsettings.json מהתיקייה הנכונה בזמן ריצה
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var keyString = config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString) || keyString.Length < 32)
                throw new Exception("JWT key is missing or too short");

            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            issuer = config["Jwt:Issuer"];
            audience = config["Jwt:Audience"];
        }

        public static SecurityToken GetToken(List<Claim> claims) =>
            new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

        public static string WriteToken(SecurityToken token) =>
            new JwtSecurityTokenHandler().WriteToken(token);
    }
}