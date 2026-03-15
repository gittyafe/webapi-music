using System;

namespace MusicWebapi.Api.Models;

public class MusicLogMessage
{
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? Action { get; set; }
    public DateTime Timestamp { get; set; }
    public int DurationTime { get; set; }
}

