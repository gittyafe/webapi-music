using MusicNameSpace.Models;

namespace MusicServices.Interfaces;

 public interface IMusicService{
       List<Music> Get();

       Music Get(int id);

       Music Create(Music newMusic);

       int Update(int id,Music newMusic);

       bool Delete(int id);

 }