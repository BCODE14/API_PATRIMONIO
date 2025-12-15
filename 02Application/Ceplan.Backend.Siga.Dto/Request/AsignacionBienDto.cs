using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Dto.Request
{
    public class AsignacionBienDtoRequest
    {
        //datos de entrada
        public string B_CODACTIVO { get; set; }
        public string? B_DESCRIP { get; set; }
        public string? B_MODE { get; set; }
        public string? B_MARCA { get; set; }
        public string? B_SERIE { get; set; }
        public string? B_ESTADO { get; set; }
        public string? B_OBS { get; set; }


    }
}
