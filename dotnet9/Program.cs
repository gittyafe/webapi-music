// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using MusicWebapi.Api.Hubs;
// using MusicWebapi.Application.Services;
// using MusicWebapi.Api.Middleware;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;




// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddMusic();
// builder.Services.AddActiveUser();
// builder.Services.AddUser();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddControllers();
// builder.Services.AddSignalR();
// builder.Services.AddRabbitMq();

// builder.Services.AddEndpointsApiExplorer();

// // Google Authentication settings
// var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
// var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

// // JWT settings
// var jwtSection = builder.Configuration.GetSection("Jwt");
// var jwtKey = jwtSection["Key"];
// var jwtIssuer = jwtSection["Issuer"];
// var jwtAudience = jwtSection["Audience"];

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = MusicWebapi.Application.Services.TokenService.GetTokenValidationParameters();
//     options.TokenValidationParameters.RoleClaimType = "role";
//     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = jwtIssuer,
//         ValidAudience = jwtAudience,
//         IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
//     };
//     options.ClaimsIssuer = "https://localhost:7159";
//     options.Events = new JwtBearerEvents
//     {
//         OnMessageReceived = context =>
//         {
//             var accessToken = context.Request.Query["access_token"];
//             var path = context.HttpContext.Request.Path;
//             if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/activityHub"))
//             {
//                 context.Token = accessToken;
//             }
//             return Task.CompletedTask;
//         }
//     };
// })
// .AddGoogle(googleOptions =>
// {
//     googleOptions.ClientId = googleClientId;
//     googleOptions.ClientSecret = googleClientSecret;
//     googleOptions.CallbackPath = "/signin-google";
// });

// // Set the default role claim type globally for authorization
// System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["role"] = "role";


// builder.Services.AddAuthorization(cfg =>
//    {
//        cfg.AddPolicy("AllUsers", policy => policy.RequireClaim("role", "Admin", "Regular"));
//        cfg.AddPolicy("Admin", policy => policy.RequireClaim("role", "Admin"));
//    });


// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Logging.ClearProviders();//log4net seriLog
// builder.Logging.AddConsole();
// // builder.Services.AddOpenApi();

// var app = builder.Build();

// app.UseMyLogMiddleware();


// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.UseSwaggerUI(options =>
//     {
//         options.SwaggerEndpoint("/openapi/v1.json", "v1");
//     });
// }

// app.UseDefaultFiles();
// app.UseStaticFiles();

// app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();
// app.MapControllers();
// app.MapHub<ActivityHub>("/activityHub");
// app.Run();


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MusicWebapi.Api.Hubs;
using MusicWebapi.Application.Services;
using MusicWebapi.Api.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddActiveUser();
builder.Services.AddMusic();
builder.Services.AddUser();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddRabbitMq();
builder.Services.AddEndpointsApiExplorer();

// JWT settings
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

Console.WriteLine($"JWT Issuer from config: {jwtIssuer}");
Console.WriteLine($"JWT Audience from config: {jwtAudience}");


if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT key is missing in configuration.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// Authentication
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateIssuerSigningKey = true,
//         ValidateLifetime = true,

//         ValidIssuer = jwtIssuer,
//         ValidAudience = jwtAudience,
//         IssuerSigningKey = signingKey,
//         RoleClaimType = "role",
//         NameClaimType = "username"
//     };

//     options.TokenValidationParameters.RoleClaimType = "role";

//     // SignalR token support
//     options.Events = new JwtBearerEvents
//     {
//         OnMessageReceived = context =>
//         {
//             var accessToken = context.Request.Query["access_token"];
//             var path = context.HttpContext.Request.Path;

//             if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/activityHub"))
//                 context.Token = accessToken;

//             return Task.CompletedTask;
//         }
//     };
// });
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["role"] = "role";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = signingKey,

        RoleClaimType = "role",
        NameClaimType = "username"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/activityHub"))
                context.Token = accessToken;

            return Task.CompletedTask;
        }
    };
});

// Authorization
builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("AllUsers", policy => policy.RequireClaim("role", "Admin", "Regular"));
    cfg.AddPolicy("Admin", policy => policy.RequireClaim("role", "Admin"));
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();

app.UseMyLogMiddleware();

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

// סדר נכון!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ActivityHub>("/activityHub");

app.Run();