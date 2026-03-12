
namespace MusicWebapi.Application.Interfaces;

public interface ICRUD<T>
{
      List<T> Get();

      T Get(int id);

      T Create(T newT);

      int Update(int id, T newT);

      bool Delete(int id);
}