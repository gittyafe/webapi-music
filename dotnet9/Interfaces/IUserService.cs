using MusicWebapi.Api.Models;

namespace MusicWebapi.Application.Interfaces;

public interface IUserService : ICRUD<User>
{
     User GetMe();
}