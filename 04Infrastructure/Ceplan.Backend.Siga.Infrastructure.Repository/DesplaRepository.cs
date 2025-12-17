
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Data;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using Dapper;

namespace Ceplan.Backend.Siga.Infraestructure.Repository
{
    public class DesplaRepository : IDesplaRepository //implementa
    {
        private readonly IConnectionFactorySqlServer _connectionFactorySqlServer; //una sola instancia de la conexion ala bbdd 

        //construtor
        public DesplaRepository(IConnectionFactorySqlServer connectionFactorySqlServer)
        {
            _connectionFactorySqlServer = connectionFactorySqlServer;
        }

        //funcion list que hace la Despla a la bbdd con la conexion creada al inicio
        public async Task<List<DesplaTempEntity>> List(DesplaTempEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionPatri()) //conexion a la bbdd
            {


                //parametros
                var parameters = new DynamicParameters();

                parameters.Add("@OP", input.op);
                parameters.Add("@Estado", input.estado);
                parameters.Add("@UO", input.uo);
                parameters.Add("@Tipo", input.tipo);
                parameters.Add("@Fecha", input.fecha);
                parameters.Add("@TipoSalida", input.tiposalida);
                parameters.Add("@MotivoSalida", input.motivosalida);
                parameters.Add("@TipoDespla", input.tipodespla);
                parameters.Add("@Trabajador", input.trabajador);




                // queryasync devuelve IEnumerable<T>
                var result = await connection.QueryAsync<DesplaTempEntity>("Patrimonio.usp_consul_lista", param: parameters, commandType: System.Data.CommandType.StoredProcedure);

                return result.AsList(); //lo convierte a List<T> 

                


            }
        }


        public Task<DesplaTempEntity> Get(DesplaTempEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Insert(DesplaTempEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Update(DesplaTempEntity input)
        {
            throw new NotImplementedException();
        }
    }
}

