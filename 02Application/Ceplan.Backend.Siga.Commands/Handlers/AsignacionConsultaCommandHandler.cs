using AutoMapper;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using MediatR;

namespace Ceplan.Backend.Siga.Application.Commands.Handlers
{
    /*
    public class AsignacionConsultaCommandHandler : IRequestHandler<AsignacionConsultaCommand, ResponseModelDto<AsignacionConsultaCommand>>
    {
        private readonly IAsignacionConsultaDomain _AsignacionDomain;

        private readonly IMapper _mapper;

        public AsignacionConsultaCommandHandler(IAsignacionConsultaDomain AsignacionConsultaDomain, IMapper mapper)
        {
            _AsignacionDomain = AsignacionDomain;
            _mapper = mapper;
        }

        public async Task<ResponseModelDto<AsignacionConsultaCommand>> Handle(AsignacionConsultaCommand request, CancellationToken cancellationToken)
        {
            var oResult = new ResponseModelDto<AsignacionConsultaCommand>() { bSuccess = true };

            //IConfig_MaeDomain _config_maeDomain = new IConfig_MaeDomain();
            //var oConfig_mae = new Config_MaeEntity();
            //oConfig_mae.id_app = request.codigo_aplicacion;
            //oConfig_mae.id_mod = request.codigo_modulo;
            //var output_cofig_mae = await this._config_maeDomain.GetbyApp(oConfig_mae);

            //request.codigo_config = output_cofig_mae.id_conf;

             var output = await _ConsultaDomain.GetById(request.IdMarcacion); //aqui hay que cambiar

            //request.codigo_creado = output.VALUE;

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

            oResult.sMessage = "registro guardado.";
            oResult.oData = request;

            return oResult;
        }
    } */

}
