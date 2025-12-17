
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Application.Dto.Response
{
    public class DesplaListDto
    {
        //datos que entrega

        public string? idsolic { get; set; }
        public string? numero_doc{ get; set; }
        public string? empleado { get; set; }
        public string? unidadorg{ get; set; }
        public string? tiposolic{ get; set; }
        public string? fechagenera{ get; set; }
        public byte[]? documento{ get; set; }
        public string? firma_usr{ get; set; }
        public string? firma_recep{ get; set; }
        public string? firma_jefe{ get; set; }
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


        public string? codbien { get; set; }
        public string? nombien { get; set; }
     

        public string? Poraprobar { get; set; }

        public string? Aprobadas { get; set; }

        public string? externo { get; set; }
        public string?  interno { get; set; }

    }
}
