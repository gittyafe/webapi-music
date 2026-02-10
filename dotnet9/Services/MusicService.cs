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
using IActiveUserN.Interfaces;
using MRepo.Services;

namespace homeWorkSe.Services;

public class MusicService : IMusicService
{
    private readonly IMusicRepository repository;
    private readonly int activeUserId;
    public MusicService(IMusicRepository repository, IActiveUser activeUser){
        this.repository=repository;
        activeUserId = activeUser.ActiveUser?.Id
                ?? throw new System.InvalidOperationException("Active user is required");
    }

    public List<Music> Get() => repository.Get().Where(m=>m.UserId == activeUserId).ToList();

    public Music Get(int id)
    {
        var music = repository.Get(id);
        return music?.UserId == activeUserId ? music : null;
    }   

    public Music Create(Music music)
    {
        music.UserId = activeUserId;
        repository.Create(music);
        return music;
    }


    public int Update(int id,Music music)
    {
        if (music.UserId != activeUserId)
            return 0;

        var existing = repository.Get(id);
        if (existing?.UserId != activeUserId)
            return 1;

        repository.Update(id,music);
        return 2;
    }

    public bool Delete(int id)
    {
        var music = Get(id);
        if (music is null || music.UserId != activeUserId)
            return false;

        repository.Delete(id);
        return true;
    }

}

public static partial class MusicExtensions
{
    public static IServiceCollection AddMusic(this IServiceCollection services)
    {
        services.AddScoped<IMusicService, MusicService>();
        services.AddSingleton<IMusicRepository, MusicRepository>();
        return services;
    }
}



