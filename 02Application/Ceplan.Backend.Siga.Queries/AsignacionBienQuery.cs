
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Dto.Request;
using MediatR;

namespace Ceplan.Backend.Siga.Application.Query
{
    //esta es una clase que debe tener un handler
    public class AsignacionBienQuery : IRequest<ResponseModelDto<byte[]>>//implementa un contrato de la interfaz donde dice lo que va a devolver 
    {
        //datos de entrada
        public string A_DNI { get; set; }
        public string? A_NOM { get; set; }
        public string? A_CORREO { get; set; }
        public string? A_CONTR { get; set; }
        public string? A_CARGO { get; set; }
        public string? A_SEDE { get; set; }
        public string? A_DIREC { get; set; }
        public string? A_CELU { get; set; }
        public string? A_UO { get; set; }
        public string? A_IPPC { get; set; }

        //EMPLE-RECIBE
        public string? R_DNI { get; set; }
        public string? R_NOM { get; set; }
        public string? R_CORREO { get; set; }
        public string? R_CONTR { get; set; }
        public string? R_CARGO { get; set; }
        public string? R_SEDE { get; set; }
        public string? R_DIREC { get; set; }
        public string? R_CELU { get; set; }
        public string? R_UO { get; set; }

        //BIENES
        public List<AsignacionBienDtoRequest> Bienes { get; set; }


        //SOLICITUD
        public string S_CODSOLIC { get; set; }
        public string? S_FECHASIG { get; set; }

        public string? S_FECHADESPLA { get; set; }
        public string? S_TIPO_DESPLA { get; set; }

        public string? S_TIPO_SALIDA { get; set; }
        public string? S_MTV_SALIDA { get; set; }



    }
}
