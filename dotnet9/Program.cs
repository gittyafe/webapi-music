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