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
    public class AsignacionConsultaProfile : Profile //hereda de profile - sirve para configurar mapeos entre clases
    {
        public AsignacionConsultaProfile()
        {
            //respuesta
            CreateMap<AsignacionConsultaEntity, AsignacionConsultaListDto>()
            .ForMember(dest => dest.codigo, opt => opt.MapFrom(src => src.codigo));

            //entrada
            CreateMap<AsignacionConsultaListQuery, AsignacionConsultaEntity>()
            .ForMember(dest => dest.cod_bien, opt => opt.MapFrom(src => src.cod_bien))
            .ForMember(dest => dest.mon_uo_asig, opt => opt.MapFrom(src => src.mon_uo_asig))
            .ForMember(dest => dest.mon_ubi_entr, opt => opt.MapFrom(src => src.mon_ubi_entr))
            .ForMember(dest => dest.dni_emple_usur_para, opt => opt.MapFrom(src => src.dni_emple_usur_para))
            .ForMember(dest => dest.motivo, opt => opt.MapFrom(src => src.motivo))
            .ForMember(dest => dest.fecha_asig, opt => opt.MapFrom(src => src.fecha_asig))
            .ForMember(dest => dest.dni_emple_resp_para, opt => opt.MapFrom(src => src.dni_emple_resp_para));

        }
    }
}
