using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Api.Models;


namespace MusicWebapi.Application.Services;

public class ActiveUserService : IActiveUser
{
    public User ActiveUser { get; private set; }
    public ActiveUserService(IHttpContextAccessor context)
    {
        var id = context?.HttpContext?.User?.FindFirst("userid")?.Value;
        var type = context?.HttpContext?.User?.FindFirst("type")?.Value;
        var name = context?.HttpContext?.User?.FindFirst("username")?.Value;

        if (id != null)
        {
            ActiveUser = new User
            {
                Id = int.Parse(id),
                Name = name,
                Type = type
            };
        }

    }

}

public static partial class MusicExtensions
{
    public static IServiceCollection AddActiveUser(this IServiceCollection services)
    {
        services.AddScoped<IActiveUser, ActiveUserService>();
        return services;
    }
}
