using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Entity
{
    public class ConsultaTempEntity
    {
        //entrada

        public int op { get; set; }
        public string? cod_activo { get; set; }
        public string? dni_emple { get; set; }

        //respuesta

        public string? codigo_activo { get; set; }
        public string? descripcion { get; set; }
        public string? modelo { get; set; }
        public string? marca { get; set; }
        public string? nro_serie { get; set; }
        public string? estado { get; set; }
        public string? fecha_asig { get; set; }
        public string? nom_completo { get; set; }
        public string? emple_final_entre { get; set; }
        public string? centro_entrega { get; set; }
        public string? emple_entrega { get; set; }
        public string? nombre { get; set; }
        public string? tipo_ubic_entre { get; set; }
        public string? cod_ubica_entre { get; set; }
    }
}
