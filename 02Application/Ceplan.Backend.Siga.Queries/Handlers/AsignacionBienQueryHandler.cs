
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
    public class AsignacionBienQueryHandler : IRequestHandler<AsignacionBienQuery, ResponseModelDto<byte[]>>

    {

        private readonly IAsignacionBienDomain _AsignacionBienDomain; //una sola instancia
        private readonly IMapper _mapper; //una sola instancia

        //construtor
        public AsignacionBienQueryHandler(IAsignacionBienDomain AsignacionBienDomain, IMapper mapper)
        {
            this._AsignacionBienDomain = AsignacionBienDomain;
            this._mapper = mapper;
        }

        
        public async Task<ResponseModelDto<byte[]>> Handle(AsignacionBienQuery request, CancellationToken cancellationToken)
        {
            ResponseModelDto<byte[]> oResponse = new() { bSuccess = true };
            
            AsignacionBienEntity oData = this._mapper.Map<AsignacionBienEntity>(request); //convierte-mapeas

            var oLista = await this._AsignacionBienDomain.AsigBien(oData); //llamas al metodo definido en domain y envias data
            
            oResponse.oData = oLista; //devuelve un solo valor

            return oResponse;
        }

    }
}
