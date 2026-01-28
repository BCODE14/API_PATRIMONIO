using System;
using System.Data;


namespace Ceplan.Backend.Siga.Domain.Entity
{
    public class AsignacionConsultaEntity : PaginatedEntity
    {
        //entrada
        public string? cod_bien { get; set; }
        public string? mon_uo_asig { get; set; }
        public string? cod_ubi_entr { get; set; }
        public string? mon_ubi_entr { get; set; }
        public string? dni_emple_usur_para { get; set; }
        public string? motivo { get; set; }
        public string? fecha_asig { get; set; }
        public string? dni_emple_resp_para { get; set; }

        //respuesta
        public string? codigo { get; set; }

    }
}
