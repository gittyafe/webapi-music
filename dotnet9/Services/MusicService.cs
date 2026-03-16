using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Services;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Api.Hubs;



namespace MusicWebapi.Application.Services;

public class MusicService : IMusicService
{
    private readonly IHubContext<ActivityHub> hubContext;
    private readonly IGenericRepository<Music> repository;
    private readonly int activeUserId;
    private readonly string? activeUsername;

    public MusicService(IGenericRepository<Music> repository, IActiveUser activeUser, IHubContext<ActivityHub> hubContext)
    {
        this.repository = repository;
        this.activeUserId = activeUser.ActiveUser?.Id
                ?? throw new System.InvalidOperationException("Active user is required");
        this.activeUsername = activeUser.ActiveUser?.Name;
        this.hubContext = hubContext;
    }

    public List<Music> Get() => repository.Get().Where(m => m.UserId == activeUserId).ToList();

    public Music Get(int id)
    {
        var music = repository.Get(id);
        if(music == null)
            return null!;
        return music;
    }

    public Music Create(Music music)
    {
        music.UserId = activeUserId;
        repository.Create(music);
        BroadcastActivity("added music", music);
        return music;
    }


    public int Update(int id, Music music)
    {
        var existing = repository.Get(id);
        if (existing?.UserId != activeUserId)
            return 1;
        music.UserId = activeUserId;
        repository.Update(id, music);
        BroadcastActivity("updated music", music);
        return 2;
    }

    public bool Delete(int id)
    {
        var music = Get(id);
        if (music is null || music.UserId != activeUserId)
            return false;

        repository.Delete(id);
        BroadcastActivity("deleted music", music);
        Console.WriteLine("BroadcastActivity called for user.Id = " + activeUserId);
        return true;
    }
    private void BroadcastActivity(string action, Music music)
    {
        hubContext.Clients.User(activeUserId.ToString()).SendAsync("ReceiveActivity", activeUsername, action, music.Name);
    }

}


public static partial class MusicExtensions
{
    public static IServiceCollection AddMusic(this IServiceCollection services)
    {
        services.AddScoped<IMusicService, MusicService>();
        services.AddSingleton<IGenericRepository<Music>, GenericRepository<Music>>();
        return services;
    }
}



