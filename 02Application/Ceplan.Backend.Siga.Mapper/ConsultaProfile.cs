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
    public class ConsultaProfile : Profile
    {
        public ConsultaProfile()
        {

            //auto mapper convertir o mapear datos entre dos clases distintas
            //mapeo de entradas - conversion de entrada 
            CreateMap<ConsultaListQuery, ConsultaTempEntity>()
              .ForMember(dest => dest.op, opt => opt.MapFrom(src => src.op))
              .ForMember(dest => dest.cod_activo, opt => opt.MapFrom(src => src.cod_activo))
              .ForMember(dest => dest.dni_emple, opt => opt.MapFrom(src => src.dni_emple));

            //mapeo de salidas
            CreateMap<ConsultaTempEntity, ConsultaListDto>()
            .ForMember(dest => dest.codigo_activo, opt => opt.MapFrom(src => src.codigo_activo))
            .ForMember(dest => dest.descripcion, opt => opt.MapFrom(src => src.descripcion))
            .ForMember(dest => dest.modelo, opt => opt.MapFrom(src => src.modelo))
            .ForMember(dest => dest.marca, opt => opt.MapFrom(src => src.marca))
            .ForMember(dest => dest.nro_serie, opt => opt.MapFrom(src => src.nro_serie))
            .ForMember(dest => dest.estado, opt => opt.MapFrom(src => src.estado))
            .ForMember(dest => dest.fecha_asig, opt => opt.MapFrom(src => src.fecha_asig))
            .ForMember(dest => dest.nom_completo, opt => opt.MapFrom(src => src.nom_completo))
            .ForMember(dest => dest.emple_final_entre, opt => opt.MapFrom(src => src.emple_final_entre))
            .ForMember(dest => dest.centro_entrega, opt => opt.MapFrom(src => src.centro_entrega))
            .ForMember(dest => dest.emple_entrega, opt => opt.MapFrom(src => src.emple_entrega))
            .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.nombre))
            .ForMember(dest => dest.tipo_ubic_entre, opt => opt.MapFrom(src => src.tipo_ubic_entre))
            .ForMember(dest => dest.cod_ubica_entre, opt => opt.MapFrom(src => src.cod_ubica_entre))
            ;

            

        }
    }
}
