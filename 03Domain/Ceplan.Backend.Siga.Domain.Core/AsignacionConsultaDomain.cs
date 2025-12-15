using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using Ceplan.Backend.Siga.Infraestructure.Interface;

namespace Ceplan.Backend.Siga.Domain.Core
{
    public class AsignacionConsultaDomain : IAsignacionConsultaDomain
    {
        private readonly IAsignacionConsultaRepository _applicationRepository; //una sola instnacia

        //construtor
        public AsignacionConsultaDomain(IAsignacionConsultaRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }
      
        //implementacion de la funcion para la asignacion - llama a repositorio
        public async Task<string> Asig(AsignacionConsultaEntity input)
        {
            return await this._applicationRepository.Asig(input);
        }

        public Task<List<AsignacionConsultaEntity>> List(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }
        
        public async Task<AsignacionConsultaEntity> Get(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseTransaccionRepository<int>> Insert(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseTransaccionRepository<int>> Update(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }

        
    }
}
