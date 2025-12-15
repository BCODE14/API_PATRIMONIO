
using Ceplan.Backend.Siga.Domain.Entity;

namespace Ceplan.Backend.Siga.Domain.Interface
{

    //dominio base para las consultas a bbdd patrimonio-definicion de funciones genericas a utilizar
    public interface IBaseDomain<T>
    {
        Task<int> Create(T input);
        Task<T> Read(T input);
        Task<int> Update(T input);
        Task<int> Delete(T input);
        Task<List<T>> ReadAllByFilter(T input);
    }

    //dominio base para la consultas a siga - definicion de funciones genericas get, insert a utilizar
    public interface IBaseDomain2<T>
    {

        Task<List<T>> List(T input); //get para primer sp
        Task<T> Get(T input);
        Task<ResponseTransaccionRepository<int>> Insert(T input);
        Task<ResponseTransaccionRepository<int>> Update(T input);

    }

}
