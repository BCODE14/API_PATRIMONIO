using Ceplan.Backend.Siga.Application.Dto.Response;
using MediatR;

namespace Ceplan.Backend.Siga.Application.Query
{
    //esta es una clase que debe tener un handler
    public class ConsultaListQuery : IRequest<ResponseModelDto<List<ConsultaListDto>>> //implementa un contrato de la interfaz donde dice lo que va a devolver 
    {
        //datos de entrada
        public int op { get; set; }

        public string? cod_activo { get; set; } 

        public string? dni_emple { get; set; }
    }
}
