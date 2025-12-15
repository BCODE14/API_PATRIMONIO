

using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using System.Collections;

namespace Ceplan.Backend.Siga.Domain.Core
{
    public class RrhhDomain : IRrhhDomain //implementa 
    {
        private readonly IRrhhRepository _applicationRepository; //una sola instancia

        public RrhhDomain(IRrhhRepository applicationRepository) //construtor
        {
            _applicationRepository = applicationRepository;
        }

        //funcion asincrona list para llamar a list del repositorio -Rrhhs siga
        public async Task<List<RrhhTempEntity>> List(RrhhTempEntity entity)
        {
            return await this._applicationRepository.List(entity); //llama a list del repository
        }


        public Task<RrhhTempEntity> Get(RrhhTempEntity input)
        {
            throw new NotImplementedException();
        }


        public Task<ResponseTransaccionRepository<int>> Insert(RrhhTempEntity input)
        {
            throw new NotImplementedException();
        }


        public Task<ResponseTransaccionRepository<int>> Update(RrhhTempEntity input)
        {
            throw new NotImplementedException();
        }

    }
}
