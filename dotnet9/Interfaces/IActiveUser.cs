using MusicWebapi.Api.Models;
using Microsoft.AspNetCore.Http;


namespace MusicWebapi.Application.Interfaces;

public interface IActiveUser
{
    User ActiveUser { get; }
}
