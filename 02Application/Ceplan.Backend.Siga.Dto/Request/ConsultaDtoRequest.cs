using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Dto.Request
{
    public class ConsultaDtoRequest
    {
        //datos de entrada
        public int op { get; set; }

        public string? cod_activo { get; set; }

        public string? dni_emple { get; set; }
    }
}
