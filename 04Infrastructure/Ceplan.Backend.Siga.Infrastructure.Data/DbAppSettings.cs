using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Infraestructure.Data
{
    public class DbAppSettings
    {        
        //definicion de campos que guardan nombre de bbdd
        public string? CeplanPatriConnection { get; set; }

        public string? CeplanSigaConnection { get; set; }
        public string? CeplanRrhhConnection { get; set; }

    }
}
