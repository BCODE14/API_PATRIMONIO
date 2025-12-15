
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
    public class RrhhListQueryHandler : IRequestHandler<RrhhListQuery, ResponseModelDto<List<RrhhListDto>>>

    {

        private readonly IRrhhDomain _RrhhDomain; //una sola instancia
        private readonly IMapper _mapper; //una sola instancia

        //construtor
        public RrhhListQueryHandler(IRrhhDomain RrhhDomain, IMapper mapper)
        {
            this._RrhhDomain = RrhhDomain;
            this._mapper = mapper;
        }

        //para Rrhhs siga 1
        public async Task<ResponseModelDto<List<RrhhListDto>>> Handle(RrhhListQuery request, CancellationToken cancellationToken)
        {
            ResponseModelDto<List<RrhhListDto>> oResponse = new() { bSuccess = true };
            oResponse.oData = new List<RrhhListDto>();


            RrhhTempEntity oData = this._mapper.Map<RrhhTempEntity>(request); //convierte-mapeas

            var oLista = await this._RrhhDomain.List(oData); //llamas al metodo definido en domain y envias data


            oResponse.sMessage = String.Format("{0} registros encontrados.", oLista.Count);

            if (oLista.Count > 0)
            {
                oResponse.oData = this._mapper.Map<List<RrhhListDto>>(oLista);
            }

            return oResponse;
        }

    }
}
