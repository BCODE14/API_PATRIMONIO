using Ceplan.Backend.Siga.Domain.Core;
using Ceplan.Backend.Siga.Domain.Interface;
using Ceplan.Backend.Siga.Infraestructure.Data;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using Ceplan.Backend.Siga.Infraestructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.gmgarcia.test_helper;

namespace Ceplan.Backend.Siga.Test.Domain
{
    [TestFixture] //indica que la clase contiene test unitario
    public class ConsultaDomainTest : TestBase
    {
        private IConsultaDomain _ConsultaDomain;

        //cargar configuraciones- leer el setting obtener el campo y inyectar como dbappsettings
        //carga la cadena de conexion para pruebas
        public override void addConfigure(ServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<DbAppSettings>(configuration.GetSection("ConnectionStrings"));
        }
        
        //registra la clase de dominio y repositorio a un entorno de pruebas

        public override void addScope(ServiceCollection services)
        {
            services.AddScoped<IConsultaDomain, ConsultaDomain>();
            services.AddScoped<IConsultaRepository, ConsultaRepository>();
        }

        //registra la conexion - abre conexion a bbdd
        public override void addSingleton(ServiceCollection services)
        {
            services.AddSingleton<IConnectionFactorySqlServer, ConnectionFactorySqlServer>();
        }

        //construtor - define el archivo de prueba a utilizar
        public ConsultaDomainTest() : base("appSetting-test.json")
        {
        }

        //se ejecuta primero y obtiene una instancia real inyectada de consulta domain para probar metodos
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this._ConsultaDomain = ServiceProvider.GetRequiredService<IConsultaDomain>();
        }

        [Test]
        public async task prueba_unitaria_sp_1()
        {
            var input = new ConsultaListQuery
            {
                op = "1",
                cod_activo = "",
                dni_emple = ""
            };

            var r = await __ConsultaDomain.List(input);

            Assert.IsNotNull(r, "sp no retorno valor");
            TestContext.Out.WriteLine("resultado:", r );

        }

    }
}
