using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
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
using MusicHubs.Hubs;

namespace homeWorkSe.Services;

public class MusicService : IMusicService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private readonly IMusicRepository repository;
    private readonly int activeUserId;
    private readonly string activeUsername;

    public MusicService(IMusicRepository repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext){
        this.repository=repository;
        this.activeUserId = activeUser.ActiveUser?.Id
                ?? throw new System.InvalidOperationException("Active user is required");
        this.activeUsername = activeUser.ActiveUser?.Name;
        this.hubContext = hubContext;
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
         BroadcastActivity("added", music);
        return music;
    }


    public int Update(int id,Music music)
    {
        var existing = repository.Get(id);
        if (existing?.UserId != activeUserId)
            return 1;
        music.UserId = activeUserId;
        repository.Update(id,music);
         BroadcastActivity("updated", music);
        return 2;
    }

    public bool Delete(int id)
    {
        var music = Get(id);
        if (music is null || music.UserId != activeUserId)
            return false;

        repository.Delete(id);
         BroadcastActivity("deleted", music);
        return true;
    }
     private void BroadcastActivity(string action, Music music)
    {
      hubContext.Clients.All.SendAsync("ReceiveActivity", activeUsername, action, music.Name);
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



