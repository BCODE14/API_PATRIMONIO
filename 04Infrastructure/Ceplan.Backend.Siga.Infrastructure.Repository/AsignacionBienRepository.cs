
using Ceplan.Backend.Siga.Domain.Entity;
using Ceplan.Backend.Siga.Infraestructure.Data;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using Dapper;
using System.Data;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Ceplan.Backend.Siga.Infraestructure.Repository
{
    public class AsignacionBienRepository : IAsignacionBienRepository
    {
        private readonly IConnectionFactorySqlServer _connectionFactorySqlServer; //una sola instancia 
        private readonly IFormatosRepository _formatos;

        //construtor
        public AsignacionBienRepository(IConnectionFactorySqlServer connectionFactorySqlServer, IFormatosRepository formatos)
        {
            _connectionFactorySqlServer = connectionFactorySqlServer;
            _formatos = formatos;
        }

        #region SOL 1 ASIGNACION
        //funcion que ejecuta asignacion en patrimonio - realiza asignacion
        public async Task<byte[]> AsigBien(AsignacionBienEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionPatri())
            {
                //consultar el numero del documento
                var sql1 = @" 
                            SELECT max(nr_nro_docu)
                            FROM PATRIMONIO.Solicitud_cab
                        ";

                var numero = await connection.QuerySingleOrDefaultAsync<int?>(sql1);

                int siguiente = (numero ?? 0) + 1;
                string nrodocu = siguiente.ToString("D5");

                input.numdoc = nrodocu;

                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //llamando a generar formato
                byte[] pdf = await _formatos.Formatouno(input);

                //GENERAR TABLA DE BIENES
                var tabla = new DataTable();
                tabla.Columns.Add("COD_BIEN", typeof(string));
                tabla.Columns.Add("DENOMINACION", typeof(string));
                tabla.Columns.Add("MARCA", typeof(string));
                tabla.Columns.Add("MODELO", typeof(string));
                tabla.Columns.Add("SERIE", typeof(string));
                tabla.Columns.Add("ESTADO", typeof(string));
                tabla.Columns.Add("OBSERVACION", typeof(string));

                foreach (var bien in input.Bienes)
                {
                    tabla.Rows.Add(
                        bien.B_CODACTIVO,
                        bien.B_DESCRIP,
                        bien.B_MARCA,
                        bien.B_MODE,
                        bien.B_SERIE,
                        bien.B_ESTADO,
                        bien.B_OBS
                    );
                }

                //guardar el registro
                var parameters = new DynamicParameters();
                parameters.Add("@A_DNI", input.A_DNI);
                parameters.Add("@A_NOM", input.A_NOM);
                parameters.Add("@A_CORREO", input.A_CORREO);
                parameters.Add("@A_CONTR", input.A_CONTR);
                parameters.Add("@A_CARGO", input.A_CARGO);
                parameters.Add("@A_SEDE", input.A_SEDE);
                parameters.Add("@A_DIREC", input.A_DIREC);
                parameters.Add("@A_CELU", input.A_CELU);
                parameters.Add("@A_UO", input.A_UO);
                parameters.Add("@A_IPPC", input.A_IPPC);

                parameters.Add("@R_DNI", input.R_DNI);
                parameters.Add("@R_NOM", input.R_NOM);
                parameters.Add("@R_CORREO", input.R_CORREO);
                parameters.Add("@R_CONTR", input.R_CONTR);
                parameters.Add("@R_CARGO", input.R_CARGO);
                parameters.Add("@R_SEDE", input.R_SEDE);
                parameters.Add("@R_DIREC", input.R_DIREC);
                parameters.Add("@R_CELU", input.R_CELU);
                parameters.Add("@R_UO", input.R_UO);
                parameters.Add("@R_ID_UBI_FSA", input.R_ID_UBI_FISICA);
                parameters.Add("@R_NOM_UBI_FSA", input.R_NOM_UBI_FISICA);

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));
                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);
                parameters.Add("@S_USUARIO ", input.S_USUARIO);

                parameters.Add("@NRODOC", nrodocu);
                parameters.Add("@NOMDOC", nomsolic);
                parameters.Add("@PDF", pdf);


                //PARAMETRO DE SALIDA RESULTADO DEL SP
                parameters.Add("@IDSOLIC", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("Patrimonio.usp_asigBien_create", parameters, commandType: CommandType.StoredProcedure);

                int result = parameters.Get<int>("@IDSOLIC");

                if (result > 0)
                {
                    // Retornar el PDF al servicio
                    return pdf;
                }
                else
                {
                    // Retornar null o lanzar excepcion
                    throw new InvalidOperationException("No se pudo registrar la asignacion de bienes.");
                }
            }
        }

        #endregion

        #region SOL 2 TELETRABAJO

        //funcion que ejecuta asignacion teletrabajo en patrimonio 
        public async Task<byte[]> Solicteletrabajo(AsignacionBienEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionPatri())
            {
                //consultar el numero del documento
                var sql1 = @" 
                            SELECT max(nr_nro_docu)
                            FROM PATRIMONIO.Solicitud_cab
                        ";

                var numero = await connection.QuerySingleOrDefaultAsync<int?>(sql1);

                int siguiente = (numero ?? 0) + 1;
                string nrodocu = siguiente.ToString("D5");

                input.numdoc = nrodocu;

                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);


                //llamando a generar formato
                byte[] pdf = await _formatos.Formatodos(input);

                //GENERAR TABLA DE BIENES
                var tabla = new DataTable();
                tabla.Columns.Add("COD_BIEN", typeof(string));
                tabla.Columns.Add("DENOMINACION", typeof(string));
                tabla.Columns.Add("MARCA", typeof(string));
                tabla.Columns.Add("MODELO", typeof(string));
                tabla.Columns.Add("SERIE", typeof(string));
                tabla.Columns.Add("ESTADO", typeof(string));
                tabla.Columns.Add("OBSERVACION", typeof(string));

                foreach (var bien in input.Bienes)
                {
                    tabla.Rows.Add(
                        bien.B_CODACTIVO,
                        bien.B_DESCRIP,
                        bien.B_MARCA,
                        bien.B_MODE,
                        bien.B_SERIE,
                        bien.B_ESTADO,
                        bien.B_OBS
                    );
                }

                //guardar el registro
                var parameters = new DynamicParameters();

                parameters.Add("@A_IPPC", input.A_IPPC);
                parameters.Add("@R_DNI", input.R_DNI);
                parameters.Add("@R_NOM", input.R_NOM);
                parameters.Add("@R_CORREO", input.R_CORREO);
                parameters.Add("@R_CONTR", input.R_CONTR);
                parameters.Add("@R_CARGO", input.R_CARGO);
                parameters.Add("@R_SEDE", input.R_SEDE);
                parameters.Add("@R_DIREC", input.R_DIREC);
                parameters.Add("@R_CELU", input.R_CELU);
                parameters.Add("@R_UO", input.R_UO);
                parameters.Add("@S_USUARIO ", input.S_USUARIO);

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));
                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);

                parameters.Add("@NRODOC", nrodocu);
                parameters.Add("@NOMDOC", nomsolic);
                parameters.Add("@PDF", pdf);

                //PARAMETRO DE SALIDA RESULTADO DEL SP
                parameters.Add("@IDSOLIC", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("Patrimonio.usp_asigTelet_create", parameters, commandType: CommandType.StoredProcedure);

                int result = parameters.Get<int>("@IDSOLIC");

                if (result > 0)
                {
                    // Retornar el PDF al servicio
                    return pdf;
                }
                else
                {
                    // Retornar null o lanzar excepcion
                    throw new InvalidOperationException("No se pudo registrar la asignacion de bienes.");
                }
            }
        }

        #endregion

        #region SOL 3 DEVOLUCION
        //funcion que ejecuta asignacion teletrabajo en patrimonio 
        public async Task<byte[]> Solicdevolucion(AsignacionBienEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionPatri())
            {
                //consultar el numero del documento
                var sql1 = @" 
                             SELECT max(nr_nro_docu)
                            FROM PATRIMONIO.Solicitud_cab
                        ";

                var numero = await connection.QuerySingleOrDefaultAsync<int?>(sql1);

                int siguiente = (numero ?? 0) + 1;
                string nrodocu = siguiente.ToString("D5");

                input.numdoc = nrodocu;

                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //llamando a generar formato
                byte[] pdf = await _formatos.Formatotres(input);

                //GENERAR TABLA DE BIENES
                var tabla = new DataTable();
                tabla.Columns.Add("COD_BIEN", typeof(string));
                tabla.Columns.Add("DENOMINACION", typeof(string));
                tabla.Columns.Add("MARCA", typeof(string));
                tabla.Columns.Add("MODELO", typeof(string));
                tabla.Columns.Add("SERIE", typeof(string));
                tabla.Columns.Add("ESTADO", typeof(string));
                tabla.Columns.Add("OBSERVACION", typeof(string));

                foreach (var bien in input.Bienes)
                {
                    tabla.Rows.Add(
                        bien.B_CODACTIVO,
                        bien.B_DESCRIP,
                        bien.B_MARCA,
                        bien.B_MODE,
                        bien.B_SERIE,
                        bien.B_ESTADO,
                        bien.B_OBS
                    );
                }

                //guardar el registro
                var parameters = new DynamicParameters();

                parameters.Add("@A_IPPC", input.A_IPPC);
                parameters.Add("@R_DNI", input.R_DNI);
                parameters.Add("@R_NOM", input.R_NOM);
                parameters.Add("@R_CORREO", input.R_CORREO);
                parameters.Add("@R_CONTR", input.R_CONTR);
                parameters.Add("@R_CARGO", input.R_CARGO);
                parameters.Add("@R_SEDE", input.R_SEDE);
                parameters.Add("@R_DIREC", input.R_DIREC);
                parameters.Add("@R_CELU", input.R_CELU);
                parameters.Add("@R_UO", input.R_UO);
                parameters.Add("@R_ID_UBI_FSA", input.R_ID_UBI_FISICA);
                parameters.Add("@R_NOM_UBI_FSA", input.R_NOM_UBI_FISICA);
                parameters.Add("@S_USUARIO ", input.S_USUARIO);

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));
                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);

                parameters.Add("@NRODOC", nrodocu);
                parameters.Add("@NOMDOC", nomsolic);
                parameters.Add("@PDF", pdf);

                //PARAMETRO DE SALIDA RESULTADO DEL SP
                parameters.Add("@IDSOLIC", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("Patrimonio.usp_devolBien_create", parameters, commandType: CommandType.StoredProcedure);

                int result = parameters.Get<int>("@IDSOLIC");

                if (result > 0)
                {
                    // Retornar el PDF al servicio
                    return pdf;
                }
                else
                {
                    // Retornar null o lanzar excepcion
                    throw new InvalidOperationException("No se pudo registrar la asignacion de bienes.");
                }
            }
        }

        #endregion

        #region SOL 4 DESPL INTERNO
        //funcion que ejecuta asignacion teletrabajo en patrimonio 
        public async Task<byte[]> Solicdesinter(AsignacionBienEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionPatri())
            {
                //consultar el numero del documento
                var sql1 = @" 
                             SELECT max(nr_nro_docu)
                            FROM PATRIMONIO.Solicitud_cab
                        ";

                var numero = await connection.QuerySingleOrDefaultAsync<int?>(sql1);

                int siguiente = (numero ?? 0) + 1;
                string nrodocu = siguiente.ToString("D5");

                input.numdoc = nrodocu;

                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //llamando a generar formato
                byte[] pdf = await _formatos.Formatocuatro(input);

                //GENERAR TABLA DE BIENES
                var tabla = new DataTable();
                tabla.Columns.Add("COD_BIEN", typeof(string));
                tabla.Columns.Add("DENOMINACION", typeof(string));
                tabla.Columns.Add("MARCA", typeof(string));
                tabla.Columns.Add("MODELO", typeof(string));
                tabla.Columns.Add("SERIE", typeof(string));
                tabla.Columns.Add("ESTADO", typeof(string));
                tabla.Columns.Add("OBSERVACION", typeof(string));

                foreach (var bien in input.Bienes)
                {
                    tabla.Rows.Add(
                        bien.B_CODACTIVO,
                        bien.B_DESCRIP,
                        bien.B_MARCA,
                        bien.B_MODE,
                        bien.B_SERIE,
                        bien.B_ESTADO,
                        bien.B_OBS
                    );
                }

                //guardar el registro
                var parameters = new DynamicParameters();
                parameters.Add("@A_DNI", input.A_DNI);
                parameters.Add("@A_NOM", input.A_NOM);
                parameters.Add("@A_CORREO", input.A_CORREO);
                parameters.Add("@A_CONTR", input.A_CONTR);
                parameters.Add("@A_CARGO", input.A_CARGO);
                parameters.Add("@A_SEDE", input.A_SEDE);
                parameters.Add("@A_DIREC", input.A_DIREC);
                parameters.Add("@A_CELU", input.A_CELU);
                parameters.Add("@A_UO", input.A_UO);
                parameters.Add("@A_IPPC", input.A_IPPC);

                parameters.Add("@R_DNI", input.R_DNI);
                parameters.Add("@R_NOM", input.R_NOM);
                parameters.Add("@R_CORREO", input.R_CORREO);
                parameters.Add("@R_CONTR", input.R_CONTR);
                parameters.Add("@R_CARGO", input.R_CARGO);
                parameters.Add("@R_SEDE", input.R_SEDE);
                parameters.Add("@R_DIREC", input.R_DIREC);
                parameters.Add("@R_CELU", input.R_CELU);
                parameters.Add("@R_UO", input.R_UO);
                parameters.Add("@S_USUARIO ", input.S_USUARIO);
                parameters.Add("@R_ID_UBI_FSA", input.R_ID_UBI_FISICA);
                parameters.Add("@R_NOM_UBI_FSA", input.R_NOM_UBI_FISICA);

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));
                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);

                parameters.Add("@S_FECHADESPL ", input.S_FECHADESPLA);
                parameters.Add("@S_TIPO_DESPLA", input.S_TIPO_DESPLA);

                parameters.Add("@NRODOC", nrodocu);
                parameters.Add("@NOMDOC", nomsolic);
                parameters.Add("@PDF", pdf);

                //PARAMETRO DE SALIDA RESULTADO DEL SP
                parameters.Add("@IDSOLIC", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("Patrimonio.usp_despIntBien_create", parameters, commandType: CommandType.StoredProcedure);

                int result = parameters.Get<int>("@IDSOLIC");

                if (result > 0)
                {
                    // Retornar el PDF al servicio
                    return pdf;
                }
                else
                {
                    // Retornar null o lanzar excepcion
                    throw new InvalidOperationException("No se pudo registrar la asignacion de bienes.");
                }
            }
        }

        #endregion

        #region SOL 5 DESPL EXTERNO
        //funcion que ejecuta asignacion teletrabajo en patrimonio 
        public async Task<byte[]> Solicdesexter(AsignacionBienEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionPatri())
            {
                //consultar el numero del documento
                var sql1 = @" 
                             SELECT max(nr_nro_docu)
                            FROM PATRIMONIO.Solicitud_cab
                        ";

                var numero = await connection.QuerySingleOrDefaultAsync<int?>(sql1);

                int siguiente = (numero ?? 0) + 1;
                string nrodocu = siguiente.ToString("D5");

                input.numdoc = nrodocu;

                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //llamando a generar formato
                byte[] pdf = await _formatos.Formatocinco(input);

                //GENERAR TABLA DE BIENES
                var tabla = new DataTable();
                tabla.Columns.Add("COD_BIEN", typeof(string));
                tabla.Columns.Add("DENOMINACION", typeof(string));
                tabla.Columns.Add("MARCA", typeof(string));
                tabla.Columns.Add("MODELO", typeof(string));
                tabla.Columns.Add("SERIE", typeof(string));
                tabla.Columns.Add("ESTADO", typeof(string));
                tabla.Columns.Add("OBSERVACION", typeof(string));

                foreach (var bien in input.Bienes)
                {
                    tabla.Rows.Add(
                        bien.B_CODACTIVO,
                        bien.B_DESCRIP,
                        bien.B_MARCA,
                        bien.B_MODE,
                        bien.B_SERIE,
                        bien.B_ESTADO,
                        bien.B_OBS
                    );
                }

                //guardar el registro
                var parameters = new DynamicParameters();
                parameters.Add("@A_DNI", input.A_DNI);
                parameters.Add("@A_NOM", input.A_NOM);
                parameters.Add("@A_CORREO", input.A_CORREO);
                parameters.Add("@A_CONTR", input.A_CONTR);
                parameters.Add("@A_CARGO", input.A_CARGO);
                parameters.Add("@A_SEDE", input.A_SEDE);
                parameters.Add("@A_DIREC", input.A_DIREC);
                parameters.Add("@A_CELU", input.A_CELU);
                parameters.Add("@A_UO", input.A_UO);
                parameters.Add("@A_IPPC", input.A_IPPC);

                parameters.Add("@R_DNI", input.R_DNI);
                parameters.Add("@R_NOM", input.R_NOM);
                parameters.Add("@R_CORREO", input.R_CORREO);
                parameters.Add("@R_CONTR", input.R_CONTR);
                parameters.Add("@R_CARGO", input.R_CARGO);
                parameters.Add("@R_SEDE", input.R_SEDE);
                parameters.Add("@R_DIREC", input.R_DIREC);
                parameters.Add("@R_CELU", input.R_CELU);
                parameters.Add("@R_UO", input.R_UO);
                parameters.Add("@S_USUARIO ", input.S_USUARIO);

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));

                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);

                parameters.Add("@S_FECHADESPL ", input.S_FECHADESPLA);
                parameters.Add("@S_TIPO_DESPLA", input.S_TIPO_DESPLA);
                parameters.Add("@S_TIPO_SALIDA", input.S_TIPO_SALIDA);
                parameters.Add("@S_MTV_SALIDA", input.S_MTV_SALIDA);

                parameters.Add("@NRODOC", nrodocu);
                parameters.Add("@NOMDOC", nomsolic);
                parameters.Add("@PDF", pdf);

                //PARAMETRO DE SALIDA RESULTADO DEL SP
                parameters.Add("@IDSOLIC", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("Patrimonio.usp_desExtBien_create", parameters, commandType: CommandType.StoredProcedure);

                int result = parameters.Get<int>("@IDSOLIC");

                if (result > 0)
                {
                    // Retornar el PDF al servicio
                    return pdf;
                }
                else
                {
                    // Retornar null o lanzar excepcion
                    throw new InvalidOperationException("No se pudo registrar la asignacion de bienes.");
                }
            }
        }
        #endregion

        #region SOL 6 DESPL ESTERNO DEVOLUCION
        //funcion que ejecuta asignacion teletrabajo en patrimonio 
        public async Task<byte[]> Solicdevobien(AsignacionBienEntity input)
        {
            using (var connection = this._connectionFactorySqlServer.GetConnectionPatri())
            {
                //consultar el numero del documento
                var sql1 = @" 
                             SELECT max(nr_nro_docu)
                            FROM PATRIMONIO.Solicitud_cab
                        ";

                var numero = await connection.QuerySingleOrDefaultAsync<int?>(sql1);

                int siguiente = (numero ?? 0) + 1;
                string nrodocu = siguiente.ToString("D5");

                input.numdoc = nrodocu;

                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //llamando a generar formato
                byte[] pdf = await _formatos.Formatoseis(input);

                //GENERAR TABLA DE BIENES
                var tabla = new DataTable();
                tabla.Columns.Add("COD_BIEN", typeof(string));
                tabla.Columns.Add("DENOMINACION", typeof(string));
                tabla.Columns.Add("MARCA", typeof(string));
                tabla.Columns.Add("MODELO", typeof(string));
                tabla.Columns.Add("SERIE", typeof(string));
                tabla.Columns.Add("ESTADO", typeof(string));
                tabla.Columns.Add("OBSERVACION", typeof(string));

                foreach (var bien in input.Bienes)
                {
                    tabla.Rows.Add(
                        bien.B_CODACTIVO,
                        bien.B_DESCRIP,
                        bien.B_MARCA,
                        bien.B_MODE,
                        bien.B_SERIE,
                        bien.B_ESTADO,
                        bien.B_OBS
                    );
                }

                //guardar el registro
                var parameters = new DynamicParameters();
                /*parameters.Add("@A_DNI", input.A_DNI);
                parameters.Add("@A_NOM", input.A_NOM);
                parameters.Add("@A_CORREO", input.A_CORREO);
                parameters.Add("@A_CONTR", input.A_CONTR);
                parameters.Add("@A_CARGO", input.A_CARGO);
                parameters.Add("@A_SEDE", input.A_SEDE);
                parameters.Add("@A_DIREC", input.A_DIREC);
                parameters.Add("@A_CELU", input.A_CELU);*/
                parameters.Add("@A_IPPC", input.A_IPPC);

                parameters.Add("@R_DNI", input.R_DNI);
                parameters.Add("@R_NOM", input.R_NOM);
                parameters.Add("@R_CORREO", input.R_CORREO);
                parameters.Add("@R_CONTR", input.R_CONTR);
                parameters.Add("@R_CARGO", input.R_CARGO);
                parameters.Add("@R_SEDE", input.R_SEDE);
                parameters.Add("@R_DIREC", input.R_DIREC);
                parameters.Add("@R_CELU", input.R_CELU);
                parameters.Add("@R_UO", input.R_UO);
                parameters.Add("@S_USUARIO ", input.S_USUARIO);

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));
                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);

                parameters.Add("@S_FECHADESPL", input.S_FECHADESPLA);
                parameters.Add("@S_TIPO_DESPLA", input.S_TIPO_DESPLA);
                parameters.Add("@S_TIPO_SALIDA", input.S_TIPO_SALIDA);
                parameters.Add("@S_MTV_SALIDA", input.S_MTV_SALIDA);

                parameters.Add("@NRODOC", nrodocu);
                parameters.Add("@NOMDOC", nomsolic);
                parameters.Add("@PDF", pdf);

                //PARAMETRO DE SALIDA RESULTADO DEL SP
                parameters.Add("@IDSOLIC", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("Patrimonio.usp_devolExt_create", parameters, commandType: CommandType.StoredProcedure);

                int result = parameters.Get<int>("@IDSOLIC");

                if (result > 0)
                {
                    // Retornar el PDF al servicio
                    return pdf;
                }
                else
                {
                    // Retornar null o lanzar excepcion
                    throw new InvalidOperationException("No se pudo registrar la asignacion de bienes.");
                }
            }
        }

        #endregion

        public Task<List<AsignacionBienEntity>> List(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }

        //para la segunda api
        public async Task<ResponseTransaccionRepository<int>> Insert(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<AsignacionBienEntity> Get(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTransaccionRepository<int>> Update(AsignacionBienEntity input)
        {
            throw new NotImplementedException();
        }
    }
}

