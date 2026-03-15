using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MusicWebapi.Api.Models;

namespace MusicWebapi.Application.Services;

public class LogWorkerService : BackgroundService
{
    private IConnection connection;
    private IChannel channel;
    private const string QueueName = "music-log";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var log = JsonSerializer.Deserialize<MusicLogMessage>(json);

            var line = $"time: {log.Timestamp:yyyy-MM-dd HH:mm:ss} | user: {log.Username} | action: {log.Action} | duration: {log.DurationTime}ms";
            await File.AppendAllTextAsync("logs.txt", line + Environment.NewLine);

            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}