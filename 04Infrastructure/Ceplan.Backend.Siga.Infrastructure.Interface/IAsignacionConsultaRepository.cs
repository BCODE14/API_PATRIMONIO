using System;
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Interface;

namespace Ceplan.Backend.Siga.Infraestructure.Interface
{
    public interface IAsignacionConsultaRepository : IBaseRepository2<AsignacionConsultaEntity>
    {

        public Task<string> Asig(AsignacionConsultaEntity input); //solo defines y lo impllementas en core
        
    }


}
