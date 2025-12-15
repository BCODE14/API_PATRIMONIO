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
    public class DesplaListQueryHandler : IRequestHandler<DesplaListQuery, ResponseModelDto<List<DesplaListDto>>>

    {

        private readonly IDesplaDomain _DesplaDomain; //una sola instancia
        private readonly IMapper _mapper; //una sola instancia

        //construtor
        public DesplaListQueryHandler(IDesplaDomain DesplaDomain, IMapper mapper)
        {
            this._DesplaDomain = DesplaDomain;
            this._mapper = mapper;
        }

        //para Desplas siga 1
        public async Task<ResponseModelDto<List<DesplaListDto>>> Handle(DesplaListQuery request, CancellationToken cancellationToken)
        {
            ResponseModelDto<List<DesplaListDto>> oResponse = new() { bSuccess = true };
            oResponse.oData = new List<DesplaListDto>();


            DesplaTempEntity oData = this._mapper.Map<DesplaTempEntity>(request); //convierte-mapeas

            var oLista = await this._DesplaDomain.List(oData); //llamas al metodo definido en domain y envias data


            oResponse.sMessage = String.Format("{0} registros encontrados.", oLista.Count);

            if (oLista.Count > 0)
            {
                oResponse.oData = this._mapper.Map<List<DesplaListDto>>(oLista);
            }

            return oResponse;
        }

    }
}
