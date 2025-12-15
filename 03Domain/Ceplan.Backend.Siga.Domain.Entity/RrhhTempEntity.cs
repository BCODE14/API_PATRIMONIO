
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Entity
{
    public class RrhhTempEntity
    {
        //entrada

        public string Dni_emple { get; set; }

        //respuesta

        //datos que entrega
        public string? A_DNI { get; set; }
        public string? A_NOM { get; set; }

        public string? A_CORREO { get; set; }

        public string? A_CONTR { get; set; }
        public string? A_CARGO { get; set; }

        public string? A_SEDE { get; set; }
        public string? A_DIREC { get; set; }

        public string? A_CELU { get; set; }
        public string? A_UO { get; set; }






    }
}
