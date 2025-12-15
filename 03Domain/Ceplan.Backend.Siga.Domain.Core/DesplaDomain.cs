using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using System.Collections;

namespace Ceplan.Backend.Siga.Domain.Core
{
    public class DesplaDomain : IDesplaDomain //implementa 
    {
        private readonly IDesplaRepository _applicationRepository; //una sola instancia

        public DesplaDomain(IDesplaRepository applicationRepository) //construtor
        {
            _applicationRepository = applicationRepository;
        }

        //funcion asincrona list para llamar a list del repositorio -Desplas siga
        public async Task<List<DesplaTempEntity>> List(DesplaTempEntity entity)
        {
            return await this._applicationRepository.List(entity); //llama a list del repository
        }


        public Task<DesplaTempEntity> Get(DesplaTempEntity input)
        {
            throw new NotImplementedException();
        }


        public Task<ResponseTransaccionRepository<int>> Insert(DesplaTempEntity input)
        {
            throw new NotImplementedException();
        }


        public Task<ResponseTransaccionRepository<int>> Update(DesplaTempEntity input)
        {
            throw new NotImplementedException();
        }

    }
}
