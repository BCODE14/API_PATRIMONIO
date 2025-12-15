using Ceplan.Backend.Siga.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Interface
{
    public interface IAsignacionConsultaDomain : IBaseDomain2<AsignacionConsultaEntity>
    {
        public Task<string> Asig(AsignacionConsultaEntity input); //solo defines y lo impllementas en core
    }
}
