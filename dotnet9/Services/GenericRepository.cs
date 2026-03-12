using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MusicWebapi.Application.Interfaces;

namespace MusicWebapi.Application.Services;

// namespace GenericRepo.Services;

// public class GenericRepository<T> : ICRUD<T>{

//     private List<T> list;

//     public GenericRepository(IWebHostEnvironment webHost)
//     {
//         this.list = new List<T>();

//         this.filePath = Path.Combine(webHost.ContentRootPath,"Data", $"{T}.json");
//         using (var jsonFile = File.OpenText(filePath))
//         {
//             var content = jsonFile.ReadToEnd();
//             list = JsonSerializer.Deserialize<List<T>>(content,
//             new JsonSerializerOptions
//             {
//                 PropertyNameCaseInsensitive = true
//             });
//         }
//     }

//     private string filePath;

//     private void saveToFile()
//     {
//         var text = JsonSerializer.Serialize(list);
//         File.WriteAllText(filePath, text);
//     }
//     private T find(int id)
//     {
//         return list.FirstOrDefault(p => p.Id == id);
//     }

//     public List<T> Get()
//     {
//         return list;
//     }


//     public T Get(int id) => find(id);

//     public T Create(T newT)
//     {
//         var maxId = list.Max(m => m.Id);
//         newT.Id = maxId + 1;
//         list.Add(newT);
//         saveToFile();
//         return newT;
//     }

//     public int Update(int id, T newT)
//     {
//         var T = find(id);
//         if (T == null)
//             return 0;
//         if (T.Id != newT.Id)
//             return 1;

//         var index = list.IndexOf(T);
//         list[index] = newT;

//         saveToFile();
//         return 2;
//     }

//     public bool Delete(int id)
//     {
//         var T = find(id);
//         if (T == null)
//             return false;
//         list.Remove(T);
//         saveToFile();
//         return true;
//     }
// }






public class GenericRepository<T> : IGenericRepository<T> where T : IEntity
{
    private readonly string filePath;
    private List<T> list;

    public GenericRepository(IWebHostEnvironment env)
    {
        filePath = Path.Combine(env.ContentRootPath, "Data", $"{typeof(T).Name}.json");

        if (!File.Exists(filePath))
        {
            list = new List<T>();
            return;
        }

        var content = File.ReadAllText(filePath);
        list = JsonSerializer.Deserialize<List<T>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
            ?? new List<T>();
    }

    private void Save() =>
        File.WriteAllText(filePath, JsonSerializer.Serialize(list));

    public List<T> Get() => list;

    public T Get(int id) => list.FirstOrDefault(x => x.Id == id);

    public T Create(T entity)
    {
        entity.Id = list.Any() ? list.Max(x => x.Id) + 1 : 1;
        list.Add(entity);
        Save();
        return entity;
    }

    public int Update(int id, T entity)
    {
        var existing = Get(id);
        if (existing == null)
            return 0;

        if (existing.Id != entity.Id)
            return 1;

        var index = list.IndexOf(existing);
        list[index] = entity;
        Save();
        return 2;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null)
            return false;

        list.Remove(entity);
        Save();
        return true;
    }
}