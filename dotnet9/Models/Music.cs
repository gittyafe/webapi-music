using MusicWebapi.Application.Interfaces;

namespace MusicWebapi.Api.Models;

public class Music : IEntity
{
    public int UserId{get;set;}
    public int Id{get;set;}
    public string Name{get;set;}
    public bool IsAccompanying{get;set;} 
}
