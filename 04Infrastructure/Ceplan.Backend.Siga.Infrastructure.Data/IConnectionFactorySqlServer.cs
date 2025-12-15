using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace Ceplan.Backend.Siga.Infraestructure.Data
{
    public interface IConnectionFactorySqlServer //defines la interfaz
    {
        IDbConnection GetConnection(string databaseName); //defines funciones
        IDbConnection GetConnectionSiga(); //defines funciones
        IDbConnection GetConnectionPatri(); //defines funciones
        IDbConnection GetConnectionRRHH();

        string GetStrConnectionSiga(); //defines funciones
    }

    //implementas la interfas - fabrica de conexiones
    public class ConnectionFactorySqlServer : IConnectionFactorySqlServer
    {
        private readonly DbAppSettings _dbAppSettings; //creas una sola instancia para toda la app

        //construtor
        public ConnectionFactorySqlServer(IOptions<DbAppSettings> dbAppSettings)
        {
            this._dbAppSettings = dbAppSettings.Value;
        }

        //funcion que te permite conexion con distintas bbdd - patrimonio por default
        public IDbConnection GetConnection(string databaseName)
        {
            var connectionString = GetConnectionString(databaseName);
            if (connectionString == null)
            {
                throw new ArgumentException($"Database name '{databaseName}' is not supported.");
            }
            var sqlConnection = new SqlConnection(); //crea la conexion
            if (sqlConnection == null)
            {
                return null;
            }
            sqlConnection.ConnectionString = connectionString;
            sqlConnection.Open(); //abre la conexion
            return sqlConnection;
        }


        //funcion que tiene conexion con una sola bbdd - siga
        public IDbConnection GetConnectionSiga()
        {
            var connectionString = _dbAppSettings.CeplanSigaConnection;
            if (connectionString == null)
            {
                throw new ArgumentException($"Database name 'siga' is not supported.");
            }
            var sqlConnection = new SqlConnection();
            if (sqlConnection == null)
            {
                return null;
            }
            sqlConnection.ConnectionString = connectionString;
            sqlConnection.Open();
            return sqlConnection;
        }

        //funcion que tiene conexion con una sola bbdd - patri
        public IDbConnection GetConnectionPatri()
        {
            var connectionString = _dbAppSettings.CeplanPatriConnection;
            if (connectionString == null)
            {
                throw new ArgumentException($"Database name 'patri' is not supported.");
            }
            var sqlConnection = new SqlConnection();
            if (sqlConnection == null)
            {
                return null;
            }
            sqlConnection.ConnectionString = connectionString;
            sqlConnection.Open();
            return sqlConnection;
        }


        public IDbConnection GetConnectionRRHH()
        {
            var connectionString = _dbAppSettings.CeplanRrhhConnection;
            if (connectionString == null)
            {
                throw new ArgumentException($"Database name 'patri' is not supported.");
            }
            var sqlConnection = new SqlConnection();
            if (sqlConnection == null)
            {
                return null;
            }
            sqlConnection.ConnectionString = connectionString;
            sqlConnection.Open();
            return sqlConnection;
        }



        public string GetStrConnectionSiga()
        {
            return _dbAppSettings.CeplanSigaConnection;
        }

        //te permite escoges entre otra base de datos
        private string GetConnectionString(string databaseName)
        {
            switch (databaseName)
            {
                case "Siga":
                    return _dbAppSettings.CeplanSigaConnection;

                default:
                    return _dbAppSettings.CeplanPatriConnection;
            }
        }
    }
}
