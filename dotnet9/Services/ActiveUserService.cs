using IActiveUserN.Interfaces;
using UserNameSpace.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace ActiveUser.Services;

public class ActiveUserService : IActiveUser
{
    public User ActiveUser { get; private set; }
    public ActiveUserService(IHttpContextAccessor context)
    {
        var userId = context?.HttpContext?.User?.FindFirst("Id");
        if (userId != null)
        {
            ActiveUser = new User
            {
                Id = int.Parse(userId.Value),
                Name = "test"
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
