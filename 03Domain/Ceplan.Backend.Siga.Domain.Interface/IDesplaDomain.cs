using Ceplan.Backend.Siga.Domain.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Interface
{
    public interface IDesplaDomain : IBaseDomain2<DesplaTempEntity>
    {
        public Task<List<DesplaTempEntity>> List(DesplaTempEntity input); //solo defines la funcion list que es un get
    }
}
