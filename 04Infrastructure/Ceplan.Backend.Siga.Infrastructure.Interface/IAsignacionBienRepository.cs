
using System;
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Interface;

namespace Ceplan.Backend.Siga.Infraestructure.Interface
{
    public interface IAsignacionBienRepository : IBaseRepository2<AsignacionBienEntity>
    {

        public Task<byte[]> AsigBien(AsignacionBienEntity input); //solo defines y lo impllementas en core
        public Task<byte[]> Solicteletrabajo(AsignacionBienEntity input);
        public Task<byte[]> Solicdevolucion(AsignacionBienEntity input);
        public Task<byte[]> Solicdesinter(AsignacionBienEntity input);
        public Task<byte[]> Solicdesexter(AsignacionBienEntity input);
        public Task<byte[]> Solicdevobien(AsignacionBienEntity input);



    }


}