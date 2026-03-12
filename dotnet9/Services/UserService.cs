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

// public class UserService : IUserService
// {

//     private List<User> list;
//     private string filePath;
//     private User activeUser;

//     public UserService(IWebHostEnvironment webHost, IActiveUser activeUser)
//     {
//         this.list = new List<User>();
//         this.activeUser = activeUser.ActiveUser;

//         this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "Users.json");
//         using (var jsonFile = File.OpenText(filePath))
//         {
//             var content = jsonFile.ReadToEnd();
//             list = JsonSerializer.Deserialize<List<User>>(content,
//             new JsonSerializerOptions
//             {
//                 PropertyNameCaseInsensitive = true
//             });
//         }
//     }


//     private void saveToFile()
//     {
//         var text = JsonSerializer.Serialize(list);
//         File.WriteAllText(filePath, text);
//     }


//     private User find(int id)
//     {
//         return list.FirstOrDefault(u => u.Id == id);
//     }

//     public List<User> Get()
//     {
//         return list;
//     }


//     public User Get(int id) => find(id);

//     public User Create(User newUser)
//     {
//         var maxId = list.Max(m => m.Id);
//         newUser.Id = maxId + 1;
//         list.Add(newUser);
//         saveToFile();
//         return newUser;
//     }

//     public int Update(int id, User newUser)
//     {
//         var usr = find(id);
//         if (usr == null)
//             return 0;
//         if (usr.Id != newUser.Id)
//             return 1;

//         var index = list.IndexOf(usr);
//         list[index] = newUser;
//         saveToFile();
//         return 2;
//     }

//     public bool Delete(int id)
//     {
//         var usr = find(id);
//         if (usr == null)
//             return false;
//         list.Remove(usr);
//         saveToFile();
//         return true;
//     }

//     // public ser GetMe(){
//     //     return Get(activeUser);
//     // }
// }

// public static class UserServiceExtension
// {
//     public static void AddUserService(this IServiceCollection services)
//     {
//         services.AddSingleton<IUserService, UserService>();
//     }
// }


public class UserService : IUserService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private readonly IGenericRepository<User> repository;
    private readonly int activeUserId;
    private readonly string activeUsername;

    public UserService(IGenericRepository<User> repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext){
        this.repository=repository;
        this.activeUserId = activeUser.ActiveUser?.Id
                ?? throw new System.InvalidOperationException("Active user is required");
        this.activeUsername = activeUser.ActiveUser?.Name;
        this.hubContext = hubContext;
    }

    public List<User> Get() => repository.Get().ToList();

    public User Get(int id)
    {
        var user = repository.Get(id);
        return user;
    }   

    public User GetMe(){
        return Get(activeUserId);
    }

    public User Create(User user)
    {
        repository.Create(user);
         BroadcastActivity("added", user);
        return user;
    }


    public int Update(int id,User user)
    {
        var existing = repository.Get(id);
        repository.Update(id,user);
        BroadcastActivity("updated", user);
        return 2;
    }

    public bool Delete(int id)
    {
        var user = Get(id);
        if (user is null)
            return false;

        repository.Delete(id);
        BroadcastActivity("deleted", user);
        return true;
    }
     private void BroadcastActivity(string action, User user)
    {
      hubContext.Clients.All.SendAsync("ReceiveActivity", activeUsername, action, user.Name);
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




