using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserNameSpace.Models;
using IUserServices.Interfaces;

namespace UserHW.Services;

public class UserService : IUserService
{

    private List<User> list;
    

    public UserService()
    {
        this.list = new List<User>{
             new User { Id = 1, Name = "Reuven",age=20},
             new User { Id = 2, Name = "Shimon",age=3},
             
        };
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

        return 2;
    }

    public bool Delete(int id)
    {
        var usr = find(id);
        if (usr == null)
            return false;
        list.Remove(usr);
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



