using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Api.Models;


namespace MusicWebapi.Application.Services;

// public class ActiveUserService : IActiveUser
// {
//     public User ActiveUser { get; private set; }
//     public ActiveUserService(IHttpContextAccessor context)
//     {
//         var id = context?.HttpContext?.User?.FindFirst("userid")?.Value;
//         var role = context?.HttpContext?.User?.FindFirst("role")?.Value;
//         var name = context?.HttpContext?.User?.FindFirst("username")?.Value;

//         if (id != null)
//         {
//             ActiveUser = new User
//             {
//                 Id = int.Parse(id),
//                 Name = name,
//                 Role = role
//             };
//         }

//     }

// }

public static partial class MusicExtensions
{
    public static IServiceCollection AddActiveUser(this IServiceCollection services)
    {
        services.AddScoped<IActiveUser, ActiveUserService>();
        return services;
    }
}
public class ActiveUserService : IActiveUser
{
    private readonly IHttpContextAccessor _context;
    private User _cachedUser;

    public ActiveUserService(IHttpContextAccessor context)
    {
        _context = context;
        _cachedUser = null!; // Suppress non-nullable warning, will be set on first access
    }

    public User ActiveUser
    {
        get
        {
            if (_cachedUser != null)
                return _cachedUser;

            var httpUser = _context.HttpContext?.User;
            var id = httpUser?.FindFirst("userid")?.Value;
            var role = httpUser?.FindFirst("role")?.Value;
            var name = httpUser?.FindFirst("username")?.Value;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(name))
                throw new InvalidOperationException("Missing required claims in JWT token.");

            _cachedUser = new User
            {
                Id = int.Parse(id),
                Name = name,
                Role = role
            };

            return _cachedUser;
        }
    }
}