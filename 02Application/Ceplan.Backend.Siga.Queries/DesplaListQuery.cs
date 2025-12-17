

using Ceplan.Backend.Siga.Application.Dto.Response;
using MediatR;

namespace Ceplan.Backend.Siga.Application.Query
{
    //esta es una clase que debe tener un handler
    public class DesplaListQuery : IRequest<ResponseModelDto<List<DesplaListDto>>> //implementa un contrato de la interfaz donde dice lo que va a devolver 
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
