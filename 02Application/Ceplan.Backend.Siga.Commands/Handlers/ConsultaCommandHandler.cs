using AutoMapper;
using Ceplan.Backend.Siga.Application.Commands;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using MediatR;

namespace Ceplan.Backend.Siga.Application.Commands.Handlers
{

    /*
    public class ConsultaCreateCommandHandler : IRequestHandler<ConsultaCommand, ResponseModelDto<ConsultaCommand>>
    {
        private readonly IConsultaDomain _ConsultaDomain; //una sola instancia

        private readonly IMapper _mapper; //una sola instancia

        //construtor
        public ConsultaCreateCommandHandler(IConsultaDomain ConsultaDomain, IMapper mapper)
        {
            _ConsultaDomain = ConsultaDomain;
            _mapper = mapper;
        }

        public async Task<ResponseModelDto<ConsultaCommand>> Handle(ConsultaCommand request, CancellationToken cancellationToken)
        {
            var oResult = new ResponseModelDto<ConsultaCommand>() { bSuccess = true };

            var output = await _ConsultaDomain.GetById(request.IdMarcacion); //aqui hay que cambiar

            if (output.STATUS_CODE == 0)
            {
                oResult.bSuccess = false;
                oResult.sMessage = output.ERROR_MESSAGE;
                return oResult;

            }

            if (output.STATUS_CODE == 2)
            {
                oResult.bSuccess = false;
                oResult.sMessage = output.ADICIONAL_DATA;
                return oResult;

            }

            oResult.sMessage = "consulta exitosa";
            oResult.oData = request;

            return oResult;
        }
    } */

}
