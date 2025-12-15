using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using System.Collections;

namespace Ceplan.Backend.Siga.Domain.Core
{
    public class ConsultaDomain : IConsultaDomain //implementa 
    {
        private readonly IConsultaRepository _applicationRepository; //una sola instancia

        public ConsultaDomain(IConsultaRepository applicationRepository) //construtor
        {
            _applicationRepository = applicationRepository;
        }

        //funcion asincrona list para llamar a list del repositorio -consultas siga
        public async Task<List<ConsultaTempEntity>> List(ConsultaTempEntity entity)
        {
            return await this._applicationRepository.List(entity); //llama a list del repository
        }


        public Task<ConsultaTempEntity> Get(ConsultaTempEntity input)
        {
            throw new NotImplementedException();
        }


        public Task<ResponseTransaccionRepository<int>> Insert(ConsultaTempEntity input)
        {
            throw new NotImplementedException();
        }


        public Task<ResponseTransaccionRepository<int>> Update(ConsultaTempEntity input)
        {
            throw new NotImplementedException();
        }
       
    }
}
