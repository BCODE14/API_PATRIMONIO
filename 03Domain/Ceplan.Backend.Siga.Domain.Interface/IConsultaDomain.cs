using Ceplan.Backend.Siga.Domain.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Interface
{
    public interface IConsultaDomain : IBaseDomain2<ConsultaTempEntity>
    {
        public Task<List<ConsultaTempEntity>> List(ConsultaTempEntity input); //solo defines la funcion list que es un get
    }
}
