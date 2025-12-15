using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Data;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using Dapper;
using System.Data;

namespace Ceplan.Backend.Siga.Infraestructure.Repository
{
    public class AsignacionConsultaRepository : IAsignacionConsultaRepository
    {
        private readonly IConnectionFactorySqlServer _connectionFactorySqlServer; //una sola instancia 

        //construtor
        public AsignacionConsultaRepository(IConnectionFactorySqlServer connectionFactorySqlServer)
        {
            _connectionFactorySqlServer = connectionFactorySqlServer;
        }

        //funcion que ejecuta la consulta en siga - realiza asignacion
        public async Task<string> Asig(AsignacionConsultaEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionSiga())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@COD_BIEN", input.cod_bien);
                parameters.Add("@NOM_UO_ASIG", input.mon_uo_asig);
                parameters.Add("@NOM_UBI_ENTR", input.mon_ubi_entr);
                parameters.Add("@DNI_EMPLE_USUR_PARA", input.dni_emple_usur_para);
                parameters.Add("@MOTIVO", input.motivo);
                parameters.Add("@FECH_ASIG", input.fecha_asig);
                parameters.Add("@DNI_EMPLE_RESP_PARA", input.dni_emple_resp_para);

                var result = await connection.ExecuteScalarAsync<string>("[sp_siga_parametros]",parameters,commandType: CommandType.StoredProcedure);

                return result;
                
            }
        }


        public Task<List<AsignacionConsultaEntity>> List(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }

        //para la segunda api
        public async Task<ResponseTransaccionRepository<int>> Insert(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<AsignacionConsultaEntity> Get(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Update(AsignacionConsultaEntity input)
        {
            throw new NotImplementedException();
        }
    }
}

