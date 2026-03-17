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
        NotifyActivity("added music", music);
        return music;
    }


    public int Update(int id, Music music)
    {
        var existing = repository.Get(id);
        if (existing?.UserId != activeUserId)
            return 1;
        music.UserId = activeUserId;
        repository.Update(id, music);
        NotifyActivity("updated music", music);
        return 2;
    }

    public bool Delete(int id)
    {
        var music = Get(id);
        if (music is null || music.UserId != activeUserId)
            return false;

        repository.Delete(id);
        NotifyActivity("deleted music", music);
        return true;
    }
    async private Task NotifyActivity(string action, Music music)
    {
        await hubContext.Clients.Group($"User_{music.UserId}").SendAsync("ReceivePersonalUpdate", action, music.Name);
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



