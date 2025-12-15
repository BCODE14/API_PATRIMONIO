using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Data;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using Dapper;

namespace Ceplan.Backend.Siga.Infraestructure.Repository
{
    public class RrhhRepository : IRrhhRepository //implementa
    {
        private readonly IConnectionFactorySqlServer _connectionFactorySqlServer; //una sola instancia de la conexion ala bbdd 

        //construtor
        public RrhhRepository(IConnectionFactorySqlServer connectionFactorySqlServer)
        {
            _connectionFactorySqlServer = connectionFactorySqlServer;
        }

        //funcion list que hace la Rrhh a la bbdd con la conexion creada al inicio
        public async Task<List<RrhhTempEntity>> List(RrhhTempEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionRRHH()) //conexion a la bbdd
            {

              
                //parametros
                var parameters = new DynamicParameters();
                parameters.Add("@DNI_EMPLE", input.Dni_emple);
               

                // queryasync devuelve IEnumerable<T>
                var result = await connection.QueryAsync<RrhhTempEntity>("[SP_RRHH_DATOS]", param: parameters, commandType: System.Data.CommandType.StoredProcedure);

                return result.AsList(); //lo convierte a List<T> 
                

              
            }
        }


        public Task<RrhhTempEntity> Get(RrhhTempEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Insert(RrhhTempEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Update(RrhhTempEntity input)
        {
            throw new NotImplementedException();
        }
    }
}

