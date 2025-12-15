using AutoMapper;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Application.Query.Handlers
{
    public class AsignacionConsultaListQueryHandler : IRequestHandler<AsignacionConsultaListQuery, ResponseModelDto<string>>
    {

        private readonly IAsignacionConsultaDomain _AsignacionConsultaDomain; //una sola instancia
        private readonly IMapper _mapper;//una sola instnacia

        //construtor
        public AsignacionConsultaListQueryHandler(IAsignacionConsultaDomain AsignacionConsultaDomain, IMapper mapper)
        {
            this._AsignacionConsultaDomain = AsignacionConsultaDomain;
            this._mapper = mapper;
        }

        //definicion del handle
        public async Task<ResponseModelDto<string>> Handle(AsignacionConsultaListQuery request, CancellationToken cancellationToken)
        {
            ResponseModelDto<string> oResponse = new() { bSuccess = true }; //respuesta exitosa

            //conversion o mapeo de compos
            AsignacionConsultaEntity oData = this._mapper.Map<AsignacionConsultaEntity>(request);

            //llamanos a la funcion list del damoin
            var oLista = await this._AsignacionConsultaDomain.Asig(oData);

            oResponse.oData = oLista; //devuelve un solo valor
        
            return oResponse;


        }

    }
}
