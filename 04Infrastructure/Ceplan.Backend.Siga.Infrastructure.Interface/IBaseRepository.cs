
using Ceplan.Backend.Siga.Domain.Entity;

namespace Ceplan.Backend.Siga.Infraestructure.Interface
{
    public interface IBaseRepository<T>
    {
        Task<int> Create(T input);
        Task<T> Read(T input);
        Task<int> Update(T input);
        Task<int> Delete(T input);
        Task<List<T>> ReadAllByFilter(T input);
    }
    public interface IBaseRepository2<T>
    {
        Task<T> Get(T input);
        Task<ResponseTransaccionRepository<int>> Insert(T input);
        Task<ResponseTransaccionRepository<int>> Update(T input);
        //Task<ResponseTransaccionRepository<int>> Delete(T input);
        Task<List<T>> List(T input);

    }
}
