using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Dto.Request
{
    public class DesplaDtoRequest
    {
        
        //datos de entrada
        public int op { get; set; }

        public string? estado{ get; set; }
        public string? uo { get; set; }
        public string? tipo { get; set; }
        public string? fecha { get; set; }

        public string? tiposalida { get; set; }
        public string? motivosalida { get; set; }
        public string? tipodespla { get; set; }
        public string? trabajador { get; set; }
    }
}
