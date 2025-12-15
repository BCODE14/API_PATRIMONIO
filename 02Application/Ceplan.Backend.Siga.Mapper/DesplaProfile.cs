

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
    public class DesplaProfile : Profile
    {
        public DesplaProfile()
        {

            //auto mapper convertir o mapear datos entre dos clases distintas
            //mapeo de entradas - conversion de entrada 
            CreateMap<DesplaListQuery, DesplaTempEntity>()
              .ForMember(dest => dest.op, opt => opt.MapFrom(src => src.op))
              .ForMember(dest => dest.estado, opt => opt.MapFrom(src => src.estado))
              .ForMember(dest => dest.uo, opt => opt.MapFrom(src => src.uo))
              .ForMember(dest => dest.tipo, opt => opt.MapFrom(src => src.tipo))
              .ForMember(dest => dest.fecha, opt => opt.MapFrom(src => src.fecha))
              .ForMember(dest => dest.tiposalida, opt => opt.MapFrom(src => src.tiposalida))
              .ForMember(dest => dest.motivosalida, opt => opt.MapFrom(src => src.motivosalida))
              .ForMember(dest => dest.tipodespla, opt => opt.MapFrom(src => src.tipodespla))
              .ForMember(dest => dest.trabajador, opt => opt.MapFrom(src => src.trabajador));

            //mapeo de salidas
            CreateMap<DesplaTempEntity, DesplaListDto>()
            .ForMember(d => d.numero_doc, opt => opt.MapFrom(s => s.numero_doc))
            .ForMember(d => d.empleado, opt => opt.MapFrom(s => s.empleado))
            .ForMember(d => d.unidadorg, opt => opt.MapFrom(s => s.unidadorg))
            .ForMember(d => d.tiposolic, opt => opt.MapFrom(s => s.tiposolic))
            .ForMember(d => d.fechagenera, opt => opt.MapFrom(s => s.fechagenera))
            .ForMember(d => d.documento, opt => opt.MapFrom(s => s.documento))
            .ForMember(d => d.firma_usr, opt => opt.MapFrom(s => s.firma_usr))
            .ForMember(d => d.firma_recep, opt => opt.MapFrom(s => s.firma_recep))
            .ForMember(d => d.firma_jefe, opt => opt.MapFrom(s => s.firma_jefe))
            .ForMember(d => d.firma_patri, opt => opt.MapFrom(s => s.firma_patri))
            .ForMember(d => d.solicestado, opt => opt.MapFrom(s => s.solicestado))

            .ForMember(d => d.B_CODACTIVO, opt => opt.MapFrom(s => s.B_CODACTIVO))
            .ForMember(d => d.B_DESCRIP, opt => opt.MapFrom(s => s.B_DESCRIP))
            .ForMember(d => d.B_MARCA, opt => opt.MapFrom(s => s.B_MARCA))
            .ForMember(d => d.B_MODE, opt => opt.MapFrom(s => s.B_MODE))
            .ForMember(d => d.B_SERIE, opt => opt.MapFrom(s => s.B_SERIE))
            .ForMember(d => d.B_ESTADO, opt => opt.MapFrom(s => s.B_ESTADO))
            .ForMember(d => d.B_UO_RECEP, opt => opt.MapFrom(s => s.B_UO_RECEP))
            .ForMember(d => d.B_EMPLE_TRANS, opt => opt.MapFrom(s => s.B_EMPLE_TRANS))
            .ForMember(d => d.B_EMPLE_RECEP, opt => opt.MapFrom(s => s.B_EMPLE_RECEP))
            .ForMember(d => d.B_FECH_DESPLA, opt => opt.MapFrom(s => s.B_FECH_DESPLA))
            .ForMember(d => d.B_TIPO_SALIDA, opt => opt.MapFrom(s => s.B_TIPO_SALIDA))
            .ForMember(d => d.B_MOTIVO_SALIDA, opt => opt.MapFrom(s => s.B_MOTIVO_SALIDA))
            .ForMember(d => d.B_ESTADO_DESPL, opt => opt.MapFrom(s => s.B_ESTADO_DESPL));

            



        }
    }
}
