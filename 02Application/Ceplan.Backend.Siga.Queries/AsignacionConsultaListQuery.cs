using Ceplan.Backend.Siga.Application.Dto.Response;
using MediatR;

namespace Ceplan.Backend.Siga.Application.Query
{
    public class AsignacionConsultaListQuery : IRequest<ResponseModelDto<string>>
    {
        //definicion de datos de entrada asignacion de bienes
        public string cod_bien { get; set; }
        public string mon_uo_asig { get; set; }
        public string mon_ubi_entr { get; set; }
        public string dni_emple_usur_para { get; set; }
        public string motivo { get; set; }
        public string fecha_asig { get; set; }
        public string? dni_emple_resp_para { get; set; }
    }
}
