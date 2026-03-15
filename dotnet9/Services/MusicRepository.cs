using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MusicNameSpace.Models;
using MusicServices.Interfaces;
using System.IO;
using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using IMusicRepo.Interfaces;

namespace MRepo.Services;

public class MusicRepository : IMusicRepository{

    private List<Music> list;

    public MusicRepository(IWebHostEnvironment webHost)
    {
        this.list = new List<Music>();

        this.filePath = Path.Combine(webHost.ContentRootPath,"Data", "Music.json");
        using (var jsonFile = File.OpenText(filePath))
        {
            var content = jsonFile.ReadToEnd();
            list = JsonSerializer.Deserialize<List<Music>>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    private string filePath;

    private void saveToFile()
    {
        var text = JsonSerializer.Serialize(list);
        File.WriteAllText(filePath, text);
    }
    private Music find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);
    }

    public List<Music> Get()
    {
        return list;
    }


    public Music Get(int id) => find(id);

    public Music Create(Music newMusic)
    {
        var maxId = list.Max(m => m.Id);
        newMusic.Id = maxId + 1;
        list.Add(newMusic);
        saveToFile();
        return newMusic;
    }

    public int Update(int id, Music newMusic)
    {
        var music = find(id);
        if (music == null)
            return 0;
        if (music.Id != newMusic.Id)
            return 1;

        var index = list.IndexOf(music);
        list[index] = newMusic;

        saveToFile();
        return 2;
    }

    public bool Delete(int id)
    {
        var music = find(id);
        if (music == null)
            return false;
        list.Remove(music);
        saveToFile();
        return true;
    }
}

