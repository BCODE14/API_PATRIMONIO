using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Entity
{
    public class DesplaTempEntity
    {
        
        //datos de entrada
        public int op { get; set; }

        public string? estado { get; set; }
        public string? uo { get; set; }
        public string? tipo { get; set; }
        public string? fecha { get; set; }

        public string? tiposalida { get; set; }
        public string? motivosalida { get; set; }
        public string? tipodespla { get; set; }
        public string? trabajador { get; set; }

        //respuesta

        //datos que entrega
        public string? numero_doc { get; set; }
        public string? empleado { get; set; }
        public string? unidadorg { get; set; }
        public string? tiposolic { get; set; }
        public string? fechagenera { get; set; }
        public byte[]? documento { get; set; }
        public string? firma_usr { get; set; }
        public string? firma_recep { get; set; }
        public string? firma_jefe { get; set; }
        public string? firma_patri { get; set; }
        public string? solicestado { get; set; }


        public string B_CODACTIVO { get; set; }
        public string? B_DESCRIP { get; set; }
        public string? B_MARCA { get; set; }
        public string? B_MODE { get; set; }
        public string? B_SERIE { get; set; }
        public string? B_ESTADO { get; set; }
        public string? B_UO_RECEP { get; set; }

        public string? B_EMPLE_TRANS { get; set; }

        public string? B_EMPLE_RECEP { get; set; }

        public string? B_FECH_DESPLA { get; set; }
        public string? B_TIPO_SALIDA { get; set; }
        public string? B_MOTIVO_SALIDA { get; set; }
        public string? B_ESTADO_DESPL { get; set; }




    }
}
