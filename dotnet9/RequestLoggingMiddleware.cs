// using MQ.Services;
// using System.Diagnostics;
// using MusicMessage.Models;
// using UserNameSpace.Models;
// using IActiveUserN.Interfaces;

// namespace MyMiddleware;

// public class MyLogMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly IRabbitMqService _producer;
//     private readonly User _activeUser;


//     public MyLogMiddleware(RequestDelegate next, IRabbitMqService producer, IActiveUser activeUser)
//     {
//         _next = next;
//         _producer = producer;
//         _activeUser = activeUser.ActiveUser;
//     }

//     public async Task Invoke(HttpContext context)
//     {
//         var sw = Stopwatch.StartNew();
//         var startTime = DateTime.Now;

//         await _next(context);

//         sw.Stop();

//         // שליפת Controller + Action
//         var endpoint = context.GetEndpoint();
//         var actionDescriptor = endpoint?.Metadata
//             .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

//         var controllerName = actionDescriptor?.ControllerName ?? "UnknownController";
//         var actionName = actionDescriptor?.ActionName ?? "UnknownAction";

//         // יצירת מודל הלוג שלך
//         var logMessage = new MusicLogMessage
//         {
//             UserId = _activeUser.Id,
//             Username = _activeUser.Name,
//             Action = $"{controllerName}/{actionName}",
//             Timestamp = startTime,
//             DurationTime = (int)sw.ElapsedMilliseconds
//         };

//         // שליחה ל-RabbitMQ
//         _producer.Publish(logMessage);
//     }
// }





// public static partial class MiddlewareExtensions
// {
//     public static IApplicationBuilder UseMyLogMiddleware(this IApplicationBuilder builder)
//     {
//         return builder.UseMiddleware<MyLogMiddleware>();
//     }
// }



using System.Diagnostics;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Services;
using MusicWebapi.Application.Interfaces;

namespace MusicWebapi.Api.Middleware;

public class MyLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRabbitMqService _producer;

    public MyLogMiddleware(RequestDelegate next, IRabbitMqService producer)
    {
        _next = next;
        _producer = producer;
    }


    public async Task Invoke(HttpContext context)
    {

        var sw = Stopwatch.StartNew();
        var startTime = DateTime.Now;
        await _next(context);
        sw.Stop();

        int userId = 0;
        string username = "Anonymous";
        try
        {
            var activeUserService = context.RequestServices.GetService<IActiveUser>();
            var activeUser = activeUserService?.ActiveUser;
            if (activeUser != null)
            {
                userId = activeUser.Id;
                username = activeUser.Name ?? "Anonymous";
            }
        }
        catch (Exception ex)
        {
            // Optionally log the exception here, but do not throw
        }

        // שליפת Controller + Action
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata
            .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

        var controllerName = actionDescriptor?.ControllerName ?? "UnknownController";
        var actionName = actionDescriptor?.ActionName ?? "UnknownAction";

        // יצירת מודל הלוג
        var logMessage = new MusicLogMessage
        {
            UserId = userId,
            Username = username,
            Action = $"{controllerName}/{actionName}",
            Timestamp = startTime,
            DurationTime = (int)sw.ElapsedMilliseconds
        };

        await _producer.Publish(logMessage);
    }
}

public static partial class MiddlewareExtensions
{
    public static IApplicationBuilder UseMyLogMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyLogMiddleware>();
    }
}
