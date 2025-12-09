using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserNameSpace.Models;
using IUserServices.Interfaces;
using System.IO;
using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;


namespace UserHW.Services;

public class UserService : IUserService
{

    private List<User> list;

    private string filePath;

    public UserService(IWebHostEnvironment webHost)
    {
        this.list = new List<User>();

        this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "Users.json");
        using (var jsonFile = File.OpenText(filePath))
        {
            var content = jsonFile.ReadToEnd();
            list = JsonSerializer.Deserialize<List<User>>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }


    private void saveToFile()
    {
        var text = JsonSerializer.Serialize(list);
        File.WriteAllText(filePath, text);
    }


    private User find(int id)
    {
        return list.FirstOrDefault(u => u.Id == id);
    }

    public List<User> Get()
    {
        return list;
    }


    public User Get(int id) => find(id);

    public User Create(User newUser)
    {
        var maxId = list.Max(m => m.Id);
        newUser.Id = maxId + 1;
        list.Add(newUser);
         saveToFile();
        return newUser;
    }

    public int Update(int id, User newUser)
    {
        var usr = find(id);
        if (usr == null)
            return 0;
        if (usr.Id != newUser.Id)
            return 1;

        var index = list.IndexOf(usr);
        list[index] = newUser;
        saveToFile();
        return 2;
    }

    public bool Delete(int id)
    {
        var usr = find(id);
        if (usr == null)
            return false;
        list.Remove(usr);
        saveToFile();
        return true;
    }
}
public static class UserServiceExtension
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserService>();
    }
}



