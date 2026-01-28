
using System;
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Interface;

namespace Ceplan.Backend.Siga.Infraestructure.Interface
{
    public interface IFormatosRepository : IBaseRepository2<AsignacionBienEntity>
    {


        public Task<byte[]> Formatouno(AsignacionBienEntity input); //solo defines y lo impllementas en core
        public Task<byte[]> Formatodos(AsignacionBienEntity input);
        public Task<byte[]> Formatotres(AsignacionBienEntity input);
        public Task<byte[]> Formatocuatro(AsignacionBienEntity input);
        public Task<byte[]> Formatocinco(AsignacionBienEntity input);
        public Task<byte[]> Formatoseis(AsignacionBienEntity input);



    }


}