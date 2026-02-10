using MusicServices.Interfaces;
using homeWorkSe.Services;
using MyMiddleware;
using UserHW.Services;
using ActiveUser.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMusic();
builder.Services.AddActiveUser();
builder.Services.AddUserService();
builder.services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Logging.ClearProviders();//log4net seriLog
builder.Logging.AddConsole(); 
// builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMyLogMiddleware();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

//////////////
app.UseDefaultFiles();
app.UseStaticFiles();
/////////////////

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
