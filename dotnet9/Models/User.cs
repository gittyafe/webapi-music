using MusicWebapi.Application.Interfaces;


namespace MusicWebapi.Api.Models;

public class User : IEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Passwd { get; set; }
    public string? Type { get; set; }
}
