
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using Ceplan.Backend.Siga.Infraestructure.Interface;

namespace Ceplan.Backend.Siga.Domain.Core
{
    public class AsignacionBienDomain : IAsignacionBienDomain
    {
        private readonly IAsignacionBienRepository _applicationRepository; //una sola instnacia

        //construtor
        public AsignacionBienDomain(IAsignacionBienRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        //implementacion de la funcion para la asignacion - llama a repositorio
        public async Task<byte[]> AsigBien(AsignacionBienEntity input)
        {
            int op = int.Parse(input.S_CODSOLIC);

            switch ( op )
            {
                case 1:
                    return await this._applicationRepository.AsigBien(input);
                    
                case 2:
                    return await this._applicationRepository.Solicteletrabajo(input);
                    
                case 3:
                    return await this._applicationRepository.Solicdevolucion(input);
                    
                case 4:
                    return await this._applicationRepository.Solicdesinter(input);
                    
                case 5:
                    return await this._applicationRepository.Solicdesexter(input);
                    
                case 6:
                    return await this._applicationRepository.Solicdevobien(input);
                    
                default:
                    throw new Exception("Operacion no valida");

            }

        }

        public Task<List<AsignacionBienEntity>> List(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }
        
        public async Task<AsignacionBienEntity> Get(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseTransaccionRepository<int>> Insert(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseTransaccionRepository<int>> Update(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }

        
    }
}
