using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Data;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using Dapper;

namespace Ceplan.Backend.Siga.Infraestructure.Repository
{
    public class ConsultaRepository : IConsultaRepository //implementa
    {
        private readonly IConnectionFactorySqlServer _connectionFactorySqlServer; //una sola instancia de la conexion ala bbdd 

        //construtor
        public ConsultaRepository(IConnectionFactorySqlServer connectionFactorySqlServer)
        {
            _connectionFactorySqlServer = connectionFactorySqlServer;
        }

        //funcion list que hace la consulta a la bbdd con la conexion creada al inicio
        public async Task<List<ConsultaTempEntity>> List(ConsultaTempEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionSiga()) //conexion a la bbdd
            {

                //parametros
                var parameters = new DynamicParameters();
                parameters.Add("@OP", input.op);
                parameters.Add("@COD_ACTIVO", input.cod_activo);
                parameters.Add("@DNI_EMPLE", input.dni_emple);

                // queryasync devuelve IEnumerable<T>
                var result = await connection.QueryAsync<ConsultaTempEntity>("[SP_SIGA_CONSULTAS]", param: parameters, commandType: System.Data.CommandType.StoredProcedure);

                return result.AsList(); //lo convierte a List<T> 



            }
        }


        public Task<ConsultaTempEntity> Get(ConsultaTempEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Insert(ConsultaTempEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Update(ConsultaTempEntity input)
        {
            throw new NotImplementedException();
        }
    }
}

