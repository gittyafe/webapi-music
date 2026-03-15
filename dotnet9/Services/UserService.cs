using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MusicWebapi.Application.Services;


public class UserService : IUserService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private readonly IGenericRepository<User> repository;
    private readonly User activeUser;
    private readonly IGenericRepository<Music> musicRepository;

    public UserService(IGenericRepository<User> repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext, IGenericRepository<Music> musicRepository)
    {
        this.repository = repository;
        this.activeUser = activeUser.ActiveUser
                ?? throw new System.InvalidOperationException("Active user is required");
        this.hubContext = hubContext;
        this.musicRepository = musicRepository;
    }

    public List<User> Get() => repository.Get().ToList();

    public User Get(int id)
    {
        if (activeUser == null)
            throw new InvalidOperationException("Active user is not set.");
        if (id != activeUser.Id && activeUser.Role != "Admin")
            return null!;
        var user = repository.Get(id);
        return user;
    }

    public User GetMe()
    {
        return Get(activeUser.Id);
    }

    public User Create(User user)
    {
        repository.Create(user);
        BroadcastActivity("added user", user);
        return user;
    }


    public int Update(int id, User user)
    {
        if (!(id == activeUser.Id || activeUser.Role == "Admin"))
            return 4; //symbolying 'Unauthorized'
        if (activeUser.Role != "Admin" && user.Role != activeUser.Role)
            return 4; //symbolying 'Unauthorized'

        var existing = repository.Get(id);
        int status = repository.Update(id, user);
        BroadcastActivity("updated user", user);
        return status;
    }

    public bool Delete(int id)
    {
        var user = Get(id);
        if (user is null)
            return false;

        musicRepository.Get().Where(m => m.UserId == id).ToList().ForEach(m => musicRepository.Delete(m.Id));
        repository.Delete(id);
        BroadcastActivity("deleted user", user);
        return true;
    }

    private void BroadcastActivity(string action, User user)
    {
        // Send only to the currently logged-in user (like music)
        hubContext.Clients.User(activeUser.Id.ToString()).SendAsync("ReceiveActivity", activeUser.Name, action, user.Name);
    }

}


public static partial class UserExtensions
{
    public static IServiceCollection AddUser(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IGenericRepository<User>, GenericRepository<User>>();
        return services;
    }
}




