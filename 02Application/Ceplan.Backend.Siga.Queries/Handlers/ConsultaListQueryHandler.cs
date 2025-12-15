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
    public class ConsultaListQueryHandler : IRequestHandler<ConsultaListQuery, ResponseModelDto<List<ConsultaListDto>>>

    {

        private readonly IConsultaDomain _ConsultaDomain; //una sola instancia
        private readonly IMapper _mapper; //una sola instancia

        //construtor
        public ConsultaListQueryHandler(IConsultaDomain ConsultaDomain, IMapper mapper)
        {
            this._ConsultaDomain = ConsultaDomain;
            this._mapper = mapper;
        }

        //para consultas siga 1
        public async Task<ResponseModelDto<List<ConsultaListDto>>> Handle(ConsultaListQuery request, CancellationToken cancellationToken)
        {
            ResponseModelDto<List<ConsultaListDto>> oResponse = new() { bSuccess = true };
            oResponse.oData = new List<ConsultaListDto>();


            ConsultaTempEntity oData = this._mapper.Map<ConsultaTempEntity>(request); //convierte-mapeas

            var oLista = await this._ConsultaDomain.List(oData); //llamas al metodo definido en domain y envias data


            oResponse.sMessage = String.Format("{0} registros encontrados.", oLista.Count);

            if (oLista.Count > 0)
            {
                oResponse.oData = this._mapper.Map<List<ConsultaListDto>>(oLista);
            }

            return oResponse;
        }

    }
}
