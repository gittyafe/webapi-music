using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Services;
using MusicWebapi.Application.Interfaces;


namespace MusicWebapi.Application.Services;

    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private IConnection connection;
        private IChannel channel;
        private const string QueueName = "music-log";

        public RabbitMqService()
        {
            InitializeAsync().GetAwaiter().GetResult();
        }

        private async System.Threading.Tasks.Task InitializeAsync()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            // Declare queue (idempotent - creates if doesn't exist)
            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,      // Survives broker restart
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public async Task Publish(MusicLogMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                body: body);
        }

        public void Dispose()
        {
            channel?.CloseAsync().Wait();
            connection?.CloseAsync().Wait();
        }
    }


    public static partial class MusicExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddHostedService<LogWorkerService>();
            return services;
        }
    }



