using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicWebapi.Api.Hubs;
using MusicWebapi.Application.Services;
using MusicWebapi.Api.Middleware;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMusic();
builder.Services.AddActiveUser();
builder.Services.AddUser();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddRabbitMq();

builder.Services.AddEndpointsApiExplorer();

///
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = MusicWebapi.Application.Services.TokenService.GetTokenValidationParameters();
        options.TokenValidationParameters.RoleClaimType = "role";
        // Also set the default claim type for roles globally
        options.ClaimsIssuer = "https://localhost:7159";

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/activityHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

// Set the default role claim type globally for authorization
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["role"] = "role";


builder.Services.AddAuthorization(cfg =>
   {
       cfg.AddPolicy("AllUsers", policy => policy.RequireClaim("role", "Admin", "Regular"));
       cfg.AddPolicy("Admin", policy => policy.RequireClaim("role", "Admin"));
   });


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

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ActivityHub>("/activityHub");
app.Run();
