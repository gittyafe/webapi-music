using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Api.Models;


namespace MusicWebapi.Application.Interfaces;

public interface IRabbitMqService
{
    Task Publish(MusicLogMessage message);
}