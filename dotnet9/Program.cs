using MusicServices.Interfaces;
using homeWorkSe.Services;
using MyMiddleware;
using UserHW.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMusicService();
builder.Services.AddUserService();
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
