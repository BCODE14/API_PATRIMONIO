using Ceplan.Backend.Siga.Application.Dto.Response;
using MediatR;

namespace Ceplan.Backend.Siga.Application.Query
{
    //esta es una clase que debe tener un handler
    public class RrhhListQuery : IRequest<ResponseModelDto<List<RrhhListDto>>> //implementa un contrato de la interfaz donde dice lo que va a devolver 
    {
        //datos de entrada
        public string? Dni_emple { get; set; }




    }
}
