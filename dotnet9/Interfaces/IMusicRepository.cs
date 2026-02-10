using MusicNameSpace.Models;

namespace IMusicRepo.Interfaces;

 public interface IMusicRepository{
       List<Music> Get();

       Music Get(int id);

       Music Create(Music newMusic);

       int Update(int id,Music newMusic);

       bool Delete(int id);

 }