


using AutoMapper;
using Ceplan.Backend.Siga.Application.Commands;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Application.Query;
using Ceplan.Backend.Siga.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Application.Mapper
{
    public class RrhhProfile : Profile
    {
        public RrhhProfile()
        {

            //auto mapper convertir o mapear datos entre dos clases distintas
            //mapeo de entradas - conversion de entrada 
            CreateMap<RrhhListQuery, RrhhTempEntity>();


            //mapeo de salidas
            CreateMap<RrhhTempEntity, RrhhListDto>();



        }
    }
}
