using UserNameSpace.Models;
using Microsoft.AspNetCore.Http;


namespace IActiveUserN.Interfaces;

public interface IActiveUser
{
    User ActiveUser { get; }
}
