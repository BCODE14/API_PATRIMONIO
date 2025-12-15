using AutoMapper;
using Ceplan.Backend.Siga.Application.Commands;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Dto.Request;
using Ceplan.Backend.Siga.Application.Query;
using Ceplan.Backend.Siga.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Application.Mapper
{
    public class AsignacionBienProfile : Profile //hereda de profile - sirve para configurar mapeos entre clases
    {
        public AsignacionBienProfile()
        {
            //respuesta
            CreateMap<AsignacionBienEntity, AsignacionBienDto>()
            .ForMember(dest => dest.Pdf, opt => opt.MapFrom(src => src.Pdf));

            //entrada
            CreateMap<AsignacionBienQuery, AsignacionBienEntity>()
            .ForMember(dest => dest.A_DNI, opt => opt.MapFrom(src => src.A_DNI))
            .ForMember(dest => dest.A_NOM, opt => opt.MapFrom(src => src.A_NOM))
            .ForMember(dest => dest.A_CORREO, opt => opt.MapFrom(src => src.A_CORREO))
            .ForMember(dest => dest.A_CONTR, opt => opt.MapFrom(src => src.A_CONTR))
            .ForMember(dest => dest.A_CARGO, opt => opt.MapFrom(src => src.A_CARGO))
            .ForMember(dest => dest.A_SEDE, opt => opt.MapFrom(src => src.A_SEDE))
            .ForMember(dest => dest.A_DIREC, opt => opt.MapFrom(src => src.A_DIREC))
            .ForMember(dest => dest.A_CELU, opt => opt.MapFrom(src => src.A_CELU))
            .ForMember(dest => dest.A_UO, opt => opt.MapFrom(src => src.A_UO))
            .ForMember(dest => dest.A_IPPC, opt => opt.MapFrom(src => src.A_IPPC))

            .ForMember(dest => dest.R_DNI, opt => opt.MapFrom(src => src.R_DNI))
            .ForMember(dest => dest.R_NOM, opt => opt.MapFrom(src => src.R_NOM))
            .ForMember(dest => dest.R_CORREO, opt => opt.MapFrom(src => src.R_CORREO))
            .ForMember(dest => dest.R_CONTR, opt => opt.MapFrom(src => src.R_CONTR))
            .ForMember(dest => dest.R_CARGO, opt => opt.MapFrom(src => src.R_CARGO))
            .ForMember(dest => dest.R_SEDE, opt => opt.MapFrom(src => src.R_SEDE))
            .ForMember(dest => dest.R_DIREC, opt => opt.MapFrom(src => src.R_DIREC))
            .ForMember(dest => dest.R_CELU, opt => opt.MapFrom(src => src.R_CELU))
            .ForMember(dest => dest.R_UO, opt => opt.MapFrom(src => src.R_UO))
            .ForMember(dest => dest.Bienes, opt => opt.MapFrom(src => src.Bienes))
            .ForMember(dest => dest.S_CODSOLIC, opt => opt.MapFrom(src => src.S_CODSOLIC))
            .ForMember(dest => dest.S_FECHASIG, opt => opt.MapFrom(src => src.S_FECHASIG))
            .ForMember(dest => dest.S_FECHADESPLA, opt => opt.MapFrom(src => src.S_FECHADESPLA))
            .ForMember(dest => dest.S_TIPO_DESPLA, opt => opt.MapFrom(src => src.S_TIPO_DESPLA))
            .ForMember(dest => dest.S_TIPO_SALIDA, opt => opt.MapFrom(src => src.S_TIPO_SALIDA))
            .ForMember(dest => dest.S_MTV_SALIDA, opt => opt.MapFrom(src => src.S_MTV_SALIDA));

            CreateMap<AsignacionBienDtoRequest, BienAsignacionEntity>()
            .ForMember(dest => dest.B_CODACTIVO, opt => opt.MapFrom(src => src.B_CODACTIVO))
            .ForMember(dest => dest.B_DESCRIP, opt => opt.MapFrom(src => src.B_DESCRIP))
            .ForMember(dest => dest.B_MODE, opt => opt.MapFrom(src => src.B_MODE))
            .ForMember(dest => dest.B_MARCA, opt => opt.MapFrom(src => src.B_MARCA))
            .ForMember(dest => dest.B_SERIE, opt => opt.MapFrom(src => src.B_SERIE))
            .ForMember(dest => dest.B_ESTADO, opt => opt.MapFrom(src => src.B_ESTADO))
            .ForMember(dest => dest.B_OBS, opt => opt.MapFrom(src => src.B_OBS));

        }
    }
}
