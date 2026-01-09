
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

        //construtor
        public AsignacionBienRepository(IConnectionFactorySqlServer connectionFactorySqlServer)
        {
            _connectionFactorySqlServer = connectionFactorySqlServer;
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


                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //generar pdf

                QuestPDF.Settings.License = LicenseType.Community;

                byte[] pdf = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(25);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Content().Column(col =>
                        {
                            col.Item().AlignCenter().Text("ANEXO N.1").Bold();
                            col.Item().AlignCenter().Text($"ASIGNACIoN EN USO DE BIENES MUEBLES PATRIMONIALESN N°{nrodocu}-\n{DateTime.Now.Year}")
                                    .Bold().FontSize(11);

                            col.Item().Height(10);

                            col.Item().Row(row =>
                            {
                                row.RelativeItem(3)
                                    .Border(1)
                                    .Padding(5)
                                    .Text("ENTIDAD U ORGANIZACIoN DE LA ENTIDAD:\nCENTRO NACIONAL DE PLANEAMIENTO ESTRATeGICO - CEPLAN")
                                    .Bold();

                                row.RelativeItem()
                                    .Border(1)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text($"FECHA:\n{DateTime.Now:dd/MM/yyyy}")
                                    .Bold();
                            });

                            col.Item().Height(10);

                            col.Item().Text("DATOS DEL USUARIO").Bold();

                            col.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(4);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(2);
                                });

                                void Cell(string text, bool bold = false)
                                {
                                    var cell = t.Cell().Border(1).Padding(4);
                                    if (bold) cell.Text(text).Bold();
                                    else cell.Text(text);
                                }

                                Cell("Nombres y apellidos", true);
                                Cell(input.R_NOM);
                                Cell("N.° DNI", true);
                                Cell(input.R_DNI);

                                Cell("Correo electronico", true);
                                Cell(input.R_CORREO);
                                Cell("Condicion contractual", true);
                                Cell(input.R_CONTR);

                                Cell("Organo o Unidad Organica", true);
                                Cell(input.R_UO, false);
                                Cell("Cargo", true);
                                Cell(input.R_CARGO);

                                Cell("Local o sede", true);
                                Cell(input.R_SEDE, false);
                                Cell("Direccion del usuario", true);
                                Cell(input.R_DIREC);
                            });

                            col.Item().Height(10);

                            col.Item().Text("DESCRIPCION DE LOS BIENES").Bold();

                            col.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(30);   // N
                                    cd.ConstantColumn(70);   // Codigo
                                    cd.RelativeColumn(3);    // Denominacion
                                    cd.RelativeColumn();     // Marca
                                    cd.RelativeColumn();     // Modelo
                                    cd.RelativeColumn();     // Serie
                                    cd.ConstantColumn(70);   // Estado
                                    cd.RelativeColumn();     // Observaciones
                                });

                                t.Header(header =>
                                {
                                    void HeaderCell(string text) =>
                                        header.Cell().Border(1).Padding(3).AlignCenter().Text(text).Bold();

                                    HeaderCell("N°");
                                    HeaderCell("Codigo patrimonial");
                                    HeaderCell("Denominacion");
                                    HeaderCell("Marca");
                                    HeaderCell("Modelo");
                                    HeaderCell("Serie");
                                    HeaderCell("Estado");
                                    HeaderCell("Observaciones");
                                });

                                int index = 1;
                                foreach (var item in input.Bienes)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(index++);
                                    t.Cell().Border(1).Padding(3).Text(item.B_CODACTIVO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_DESCRIP);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MARCA);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MODE);
                                    t.Cell().Border(1).Padding(3).Text(item.B_SERIE);
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(item.B_ESTADO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_OBS);
                                }

                               
                               
                            });

                            col.Item().Height(10);
                            col.Item().Text("CONSIDERACIONES:").Bold();

                            col.Item().Border(1).Padding(6).Text(
                                "El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, " +
                                "debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc.\n\n" +
                                "Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, " +
                                "es previamente comunicado al Especialista en Gestion Patrimonial y Almacen."
                            );

                            col.Item().Height(20);
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().Text("Quien recibe").Bold();
                                    c.Item().Height(30);
                                    c.Item().Text("______________________________");
                                    c.Item().Text("Usuario").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().Text("Quien entrega").Bold();
                                    c.Item().Height(30);
                                    c.Item().Text("______________________________");
                                    c.Item().Text("Patrimonio").Bold();
                                });
                            });
                        });
                    });
                }).GeneratePdf();


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

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));
                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);
                parameters.Add("@S_FECHADESPL ", input.S_USUARIO);

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


                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //generar pdf

                QuestPDF.Settings.License = LicenseType.Community;

                byte[] pdf = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(25);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Content().Column(col =>
                        {
                            col.Item().AlignCenter().Text("ANEXO N.° 1").Bold();
                            col.Item().AlignCenter().Text($"ASIGNACION EN USO DE BIENES MUEBLES PATRIMONIALESN-TELETRABAJO N° {nrodocu}-\n{DateTime.Now.Year}")
                                .Bold().FontSize(11);

                            col.Item().Height(10);

                            col.Item().Row(row =>
                            {
                                row.RelativeItem(3)
                                    .Border(1)
                                    .Padding(5)
                                    .Text("ENTIDAD U ORGANIZACIoN DE LA ENTIDAD:\nCENTRO NACIONAL DE PLANEAMIENTO ESTRATeGICO - CEPLAN")
                                    .Bold();

                                row.RelativeItem()
                                    .Border(1)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text($"FECHA:\n{DateTime.Now:dd/MM/yyyy}")
                                    .Bold();
                            });

                            col.Item().Height(10);

                            col.Item().Text("DATOS DEL USUARIO").Bold();

                            col.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(4);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(2);
                                });

                                void Cell(string text, bool bold = false)
                                {
                                    var cell = t.Cell().Border(1).Padding(4);
                                    if (bold) cell.Text(text).Bold();
                                    else cell.Text(text);
                                }

                                Cell("Nombres y apellidos", true);
                                Cell(input.R_NOM);
                                Cell("N.° DNI", true);
                                Cell(input.R_DNI);

                                Cell("Correo electronico", true);
                                Cell(input.R_CORREO);
                                Cell("Condicion contractual", true);
                                Cell(input.R_CONTR);

                                Cell("Organo o Unidad Organica", true);
                                Cell(input.R_UO, false);
                                Cell("Cargo", true);
                                Cell(input.R_CARGO);

                                Cell("Local o sede", true);
                                Cell(input.R_SEDE, false);
                                Cell("Direccion del usuario", true);
                                Cell(input.R_DIREC);
                            });

                            col.Item().Height(10);

                            col.Item().Text("DESCRIPCION DE LOS BIENES").Bold();

                            col.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(30);   // N
                                    cd.ConstantColumn(70);   // Codigo
                                    cd.RelativeColumn(3);    // Denominacion
                                    cd.RelativeColumn();     // Marca
                                    cd.RelativeColumn();     // Modelo
                                    cd.RelativeColumn();     // Serie
                                    cd.ConstantColumn(70);   // Estado
                                    cd.RelativeColumn();     // Observaciones
                                });

                                void Header(string text)
                                    => t.Cell().Border(1).Padding(3).AlignCenter().Text(text).Bold();

                                Header("N°");
                                Header("Codigo patrimonial");
                                Header("Denominacion");
                                Header("Marca");
                                Header("Modelo");
                                Header("Serie");
                                Header("Estado");
                                Header("Observaciones");

                                int index = 1;
                                foreach (var item in input.Bienes)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(index++);
                                    t.Cell().Border(1).Padding(3).Text(item.B_CODACTIVO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_DESCRIP);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MARCA);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MODE);
                                    t.Cell().Border(1).Padding(3).Text(item.B_SERIE);
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(item.B_ESTADO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_OBS);
                                }

                                // Completar hasta 10 filas
                                for (int i = index; i <= 10; i++)
                                {
                                    for (int j = 0; j < 9; j++)
                                        t.Cell().Border(1).Padding(10).Text("");
                                }
                            });

                            col.Item().Height(10);
                            col.Item().Text("CONSIDERACIONES:").Bold();

                            col.Item().Border(1).Padding(6).Text(
                                "El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, " +
                                "debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc.\n\n" +
                                "Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, " +
                                "es previamente comunicado al Especialista en Gestion Patrimonial y Almacen."
                            );

                            col.Item().Height(20);
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().Text("Quien recibe").Bold();
                                    c.Item().Height(30);
                                    c.Item().Text("______________________________");
                                    c.Item().Text("Usuario").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().Text("Quien entrega").Bold();
                                    c.Item().Height(30);
                                    c.Item().Text("______________________________");
                                    c.Item().Text("Patrimonio").Bold();
                                });
                            });
                        });
                    });
                }).GeneratePdf();




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


                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //generar pdf

                QuestPDF.Settings.License = LicenseType.Community;

                byte[] pdf = Document.Create(container =>
                                {
                                    container.Page(page =>
                                    {
                                        page.Size(PageSizes.A4);
                                        page.Margin(25);
                                        page.DefaultTextStyle(x => x.FontSize(9));

                                        page.Content().Column(col =>
                                        {
                                            col.Item().AlignCenter().Text("ANEXO N.° 1").Bold();
                                            col.Item().AlignCenter().Text($"DEVOLUCION DE BIENES INMUEBLES PATRIMONIALESN N°{nrodocu}-\n{DateTime.Now.Year}")
                                                .Bold().FontSize(11);

                                            col.Item().Height(10);

                                            col.Item().Row(row =>
                                            {
                                                row.RelativeItem(3)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text("ENTIDAD U ORGANIZACION DE LA ENTIDAD:\nCENTRO NACIONAL DE PLANEAMIENTO ESTRATeGICO - CEPLAN")
                                                    .Bold();

                                                row.RelativeItem()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .AlignCenter()
                                                    .Text($"FECHA:\n{DateTime.Now:dd/MM/yyyy}")
                                                    .Bold();
                                            });

                                            col.Item().Height(10);

                                            col.Item().Text("DATOS DEL USUARIO").Bold();

                                            col.Item().Border(1).Table(t =>
                                            {
                                                t.ColumnsDefinition(cd =>
                                                {
                                                    cd.RelativeColumn(2);
                                                    cd.RelativeColumn(4);
                                                    cd.RelativeColumn(2);
                                                    cd.RelativeColumn(2);
                                                });

                                                void Cell(string text, bool bold = false)
                                                {
                                                    var cell = t.Cell().Border(1).Padding(4);
                                                    if (bold) cell.Text(text).Bold();
                                                    else cell.Text(text);
                                                }

                                                Cell("Nombres y apellidos", true);
                                                Cell(input.R_NOM);
                                                Cell("N.° DNI", true);
                                                Cell(input.R_DNI);

                                                Cell("Correo electronico", true);
                                                Cell(input.R_CORREO);
                                                Cell("Condicion contractual", true);
                                                Cell(input.R_CONTR);

                                                Cell("Organo o Unidad Organica", true);
                                                Cell(input.R_UO, false);
                                                Cell("Cargo", true);
                                                Cell(input.R_CARGO);

                                                Cell("Local o sede", true);
                                                Cell(input.R_SEDE, false);
                                                Cell("Direccion del usuario", true);
                                                Cell(input.R_DIREC);
                                            });

                                            col.Item().Height(10);

                                            col.Item().Text("DESCRIPCION DE LOS BIENES").Bold();

                                            col.Item().Border(1).Table(t =>
                                            {
                                                t.ColumnsDefinition(cd =>
                                                {
                                                    cd.ConstantColumn(30);   // N
                                                    cd.ConstantColumn(70);   // Codigo
                                                    cd.RelativeColumn(3);    // Denominacion
                                                    cd.RelativeColumn();     // Marca
                                                    cd.RelativeColumn();     // Modelo
                                                    cd.RelativeColumn();     // Serie
                                                    cd.ConstantColumn(70);   // Estado
                                                    cd.RelativeColumn();     // Observaciones
                                                });

                                                void Header(string text)
                                                    => t.Cell().Border(1).Padding(3).AlignCenter().Text(text).Bold();

                                                Header("N°");
                                                Header("Codigo patrimonial");
                                                Header("Denominacion");
                                                Header("Marca");
                                                Header("Modelo");
                                                Header("Serie");
                                                Header("Estado");
                                                Header("Observaciones");

                                                int index = 1;
                                                foreach (var item in input.Bienes)
                                                {
                                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(index++);
                                                    t.Cell().Border(1).Padding(3).Text(item.B_CODACTIVO);
                                                    t.Cell().Border(1).Padding(3).Text(item.B_DESCRIP);
                                                    t.Cell().Border(1).Padding(3).Text(item.B_MARCA);
                                                    t.Cell().Border(1).Padding(3).Text(item.B_MODE);
                                                    t.Cell().Border(1).Padding(3).Text(item.B_SERIE);
                                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(item.B_ESTADO);
                                                    t.Cell().Border(1).Padding(3).Text(item.B_OBS);
                                                }

                                                // Completar hasta 10 filas
                                                for (int i = index; i <= 10; i++)
                                                {
                                                    for (int j = 0; j < 9; j++)
                                                        t.Cell().Border(1).Padding(10).Text("");
                                                }
                                            });

                                            col.Item().Height(10);
                                            col.Item().Text("CONSIDERACIONES:").Bold();

                                            col.Item().Border(1).Padding(6).Text(
                                                "El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, " +
                                                "debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc.\n\n" +
                                                "Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, " +
                                                "es previamente comunicado al Especialista en Gestion Patrimonial y Almacen."
                                            );

                                            col.Item().Height(20);
                                            col.Item().Row(row =>
                                            {
                                                row.RelativeItem().AlignCenter().Column(c =>
                                                {
                                                    c.Item().Text("Quien entrega:").Bold();
                                                    c.Item().Height(30);
                                                    c.Item().Text("______________________________");
                                                    c.Item().Text("Usuario").Bold();
                                                });

                                                row.RelativeItem().AlignCenter().Column(c =>
                                                {
                                                    c.Item().Text("Quien recibe:").Bold();
                                                    c.Item().Height(30);
                                                    c.Item().Text("______________________________");
                                                    c.Item().Text("Patrimonio").Bold();
                                                });
                                            });
                                        });
                                    });
                                }).GeneratePdf();

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


                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //generar pdf

                QuestPDF.Settings.License = LicenseType.Community;


                byte[] pdf = QuestPDF.Fluent.Document.Create(d =>
                    d.Page(p =>
                    {
                        p.Size(QuestPDF.Helpers.PageSizes.A4);
                        p.Margin(25);

                        p.Content().Column(c =>
                        {
                            c.Item().AlignCenter().Text("ANEXO N. 3").Bold();
                            c.Item().AlignCenter()
                                .Text($"DESPLAZAMIENTO INTERNO DE BIENES MUEBLES PATRIMONIALES N° {nrodocu} -\n{DateTime.Now.Year}")
                                .Bold()
                                .FontSize(11);

                            c.Item().Height(10);

                            c.Item().Border(1).Padding(6).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Cell().Text("ENTIDAD U ORGANIZACION").Bold();
                                t.Cell().Text("FECHA").Bold();

                                t.Cell().Text("CENTRO NACIONAL DE PLANEAMIENTO ESTRATEGICO - CEPLAN");
                                t.Cell().Text(DateTime.Now.ToString("dd/MM/yyyy"));
                            });

                            c.Item().Height(8);

                            c.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                });

                                t.Cell().Padding(5).Text("USUARIO QUE TRANSFIERE").Bold();
                                t.Cell().Padding(5).Text("USUARIO QUE RECEPCIONA").Bold();

                                t.Cell().Padding(5).Text("Nombres y Apellidos:");
                                t.Cell().Padding(5).Text("Nombres y Apellidos:");

                                t.Cell().Padding(5).Text(input.A_NOM);
                                t.Cell().Padding(5).Text(input.R_NOM);

                                t.Cell().Padding(5).Text("DNI:");
                                t.Cell().Padding(5).Text("DNI:");

                                t.Cell().Padding(5).Text(input.A_DNI);
                                t.Cell().Padding(5).Text(input.R_DNI);

                                t.Cell().Padding(5).Text("Organo o Unidad Organica:");
                                t.Cell().Padding(5).Text("Organo o Unidad Organica:");

                                t.Cell().Padding(5).Text(input.A_UO);
                                t.Cell().Padding(5).Text(input.R_UO);

                                t.Cell().Padding(5).Text("Local o Sede:");
                                t.Cell().Padding(5).Text("Local o Sede:");

                                t.Cell().Padding(5).Text(input.A_SEDE);
                                t.Cell().Padding(5).Text(input.R_SEDE);
                            });

                            c.Item().Height(10);

                            c.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(25);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn(1.5f);
                                });

                                void Header(string text)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(text).Bold();
                                }

                                Header("N°");
                                Header("Codigo Patrimonial");
                                Header("Denominacion");
                                Header("Marca");
                                Header("Modelo");
                                Header("Serie");
                                Header("Estado");
                                Header("Observaciones");

                                int index = 1;
                                foreach (var item in input.Bienes)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(index++);
                                    t.Cell().Border(1).Padding(3).Text(item.B_CODACTIVO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_DESCRIP);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MARCA);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MODE);
                                    t.Cell().Border(1).Padding(3).Text(item.B_SERIE);
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(item.B_ESTADO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_OBS);
                                }

                            });

                            c.Item().Height(15);

                            c.Item().Row(r =>
                            {
                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter().Text("Quien entrega");
                                });

                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter().Text("Quien recibe");
                                });

                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter().Text("Visto Bueno - Patrimonio");
                                });
                            });
                        });
                    })
                ).GeneratePdf();

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


                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //generar pdf

                QuestPDF.Settings.License = LicenseType.Community;


                byte[] pdf = QuestPDF.Fluent.Document.Create(d =>
                    d.Page(p =>
                    {
                        p.Size(QuestPDF.Helpers.PageSizes.A4);
                        p.Margin(25);
                        p.DefaultTextStyle(x => x.FontSize(9));

                        p.Content().Column(c =>
                        {
                            c.Item().AlignCenter().Text("ANEXO N 4").Bold();
                            c.Item().AlignCenter()
                                .Text($"ORDEN DE SALIDA Y REINGRESO DE BIENES MUEBLES PATRIMONIALES N° {nrodocu}-\n{DateTime.Now.Year}")
                                .Bold();

                            c.Item().Height(10);
                            c.Item().Border(1).Padding(5).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Cell().Text("ENTIDAD U ORGANIZACION DE LA ENTIDAD").Bold();
                                t.Cell().Text("FECHA").Bold();

                                t.Cell().Text("CENTRO NACIONAL DE PLANEAMIENTO ESTRATEGICO - CEPLAN");
                                t.Cell().Text($"{DateTime.Now:dd/MM/yyyy}");
                            });

                            c.Item().Height(6);

                            c.Item().Border(1).Padding(5).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                });

                                t.Cell().Text("TIPO").Bold();
                                int tipoSalida = int.TryParse(input.S_TIPO_SALIDA, out int value) ? value : 0;
                                t.Cell().Text(tipoSalida == 1 ? "✔ SALIDA" : "SALIDA");
                                t.Cell().Text(tipoSalida == 2 ? "✔ REINGRESO" : "REINGRESO");

                            });

                            c.Item().Height(6);

                            c.Item().Border(1).Padding(5).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(60);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                });

                                t.Cell().Text("MOTIVO").Bold();
                                string motivo = input.S_MTV_SALIDA;
                                t.Cell().Text(motivo == "1" ? "✔ MANTENIMIENTO" : "MANTENIMIENTO");
                                t.Cell().Text(motivo == "2" ? "✔ COMISIoN DE SERVICIOS" : "COMISIoN DE SERVICIOS");
                                t.Cell().Text(motivo == "3" ? "✔ CAPACITACIoN Y/O EVENTO" : "CAPACITACIoN Y/O EVENTO");

                            });

                            c.Item().Height(8);

                            c.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                });

                                t.Cell().Padding(4).Text("DATOS DEL RESPONSABLE DEL TRASLADO").Bold();
                                t.Cell().Padding(4).Text("DESTINO").Bold();

                                t.Cell().Padding(4).Text($"Nombres y Apellidos: {input.A_NOM}");
                                t.Cell().Padding(4).Text($"Representante / Proveedor / Usuario: {input.R_NOM}");

                                t.Cell().Padding(4).Text($"DNI: {input.A_DNI}");
                                t.Cell().Padding(4).Text($"DNI / RUC: {input.R_DNI}");

                                t.Cell().Padding(4).Text($"Correo: {input.A_CORREO}");
                                t.Cell().Padding(4).Text($"Direccion: {input.R_DIREC}");

                                t.Cell().Padding(4).Text($"Organo / Unidad Organica: {input.A_UO}");
                                t.Cell().Padding(4).Text($"Proveedor / organo: {input.R_UO}");

                                t.Cell().Padding(4).Text($"Local o sede: {input.A_SEDE}");
                                t.Cell().Padding(4).Text($"Local o sede: {input.R_SEDE}");
                            });

                            c.Item().Height(10);

                            c.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(25);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn(1.5f);
                                });

                                void Header(string text)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(text).Bold();
                                }

                                Header("N°");
                                Header("Codigo Patrimonial");
                                Header("Denominacion");
                                Header("Marca");
                                Header("Modelo");
                                Header("Serie");
                                Header("Estado");
                                Header("Observaciones");

                                int index = 1;
                                foreach (var item in input.Bienes)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(index++);
                                    t.Cell().Border(1).Padding(3).Text(item.B_CODACTIVO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_DESCRIP);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MARCA);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MODE);
                                    t.Cell().Border(1).Padding(3).Text(item.B_SERIE);
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(item.B_ESTADO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_OBS);
                                }

                            });

                            c.Item().Height(20);

                            c.Item().Row(r =>
                            {
                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter().Text("Responsable del Traslado");
                                });

                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter()
                                        .Text("Jefe de la Direccion u Oficina que autoriza el traslado");
                                });

                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter().Text("Visto Bueno Patrimonio");
                                });
                            });
                        });
                    })
                ).GeneratePdf();


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


                //nombre de la solicitu

                var sql2 = @" 
                            SELECT vc_nombre
                            FROM PATRIMONIO.Parametro_ref 
                            where nr_cod_detalle = @S_CODSOLIC and vc_cod_maestro = 'TIPO_SOLIC'
                        ";

                var entrada = new DynamicParameters();
                entrada.Add("@S_CODSOLIC", input.S_CODSOLIC);

                var nomsolic = await connection.QuerySingleOrDefaultAsync<string>(sql2, entrada);

                //generar pdf

                QuestPDF.Settings.License = LicenseType.Community;

                byte[] pdf = QuestPDF.Fluent.Document.Create(d =>
                    d.Page(p =>
                    {
                        p.Size(QuestPDF.Helpers.PageSizes.A4);
                        p.Margin(25);
                        p.DefaultTextStyle(x => x.FontSize(9));

                        p.Content().Column(c =>
                        {
                            c.Item().AlignCenter().Text("ANEXO N. 4").Bold();
                            c.Item().AlignCenter()
                                .Text($"ORDEN DE SALIDA Y REINGRESO DE BIENES MUEBLES PATRIMONIALES N. {nrodocu}-\n{DateTime.Now.Year}")
                                .Bold();

                            c.Item().Height(10);
                            c.Item().Border(1).Padding(5).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Cell().Text("ENTIDAD U ORGANIZACION DE LA ENTIDAD").Bold();
                                t.Cell().Text("FECHA").Bold();

                                t.Cell().Text("CENTRO NACIONAL DE PLANEAMIENTO ESTRATEGICO - CEPLAN");
                                t.Cell().Text($"{DateTime.Now:dd/MM/yyyy}");
                            });

                            c.Item().Height(6);

                            c.Item().Border(1).Padding(5).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                });


                                t.Cell().Text("TIPO").Bold();
                                int tipoSalida = int.TryParse(input.S_TIPO_SALIDA, out int value) ? value : 0;
                                t.Cell().Text(tipoSalida == 1 ? "✔ SALIDA" : "SALIDA");
                                t.Cell().Text(tipoSalida == 2 ? "✔ REINGRESO" : "REINGRESO");
                            });

                            c.Item().Height(6);

                            c.Item().Border(1).Padding(5).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(60);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                });


                                t.Cell().Text("MOTIVO").Bold();
                                string motivo = input.S_MTV_SALIDA;
                                t.Cell().Text(motivo == "1" ? "✔ MANTENIMIENTO" : "MANTENIMIENTO");
                                t.Cell().Text(motivo == "2" ? "✔ COMISIoN DE SERVICIOS" : "COMISIoN DE SERVICIOS");
                                t.Cell().Text(motivo == "3" ? "✔ CAPACITACIoN Y/O EVENTO" : "CAPACITACIoN Y/O EVENTO");
                            });

                            c.Item().Height(8);

                            c.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                });

                                t.Cell().Padding(4).Text("DATOS DEL RESPONSABLE DEL TRASLADO").Bold();
                                t.Cell().Padding(4).Text("DESTINO").Bold();

                                t.Cell().Padding(4).Text($"Nombres y Apellidos: {input.A_NOM}");
                                t.Cell().Padding(4).Text($"Representante / Proveedor / Usuario: {input.R_NOM}");

                                t.Cell().Padding(4).Text($"DNI: {input.A_DNI}");
                                t.Cell().Padding(4).Text($"DNI / RUC: {input.R_DNI}");

                                t.Cell().Padding(4).Text($"Correo: {input.A_CORREO}");
                                t.Cell().Padding(4).Text($"Direccion: {input.R_DIREC}");

                                t.Cell().Padding(4).Text($"Organo / Unidad Organica: {input.A_UO}");
                                t.Cell().Padding(4).Text($"Proveedor / organo: {input.R_UO}");

                                t.Cell().Padding(4).Text($"Local o sede: {input.A_SEDE}");
                                t.Cell().Padding(4).Text($"Local o sede: {input.R_SEDE}");
                            });

                            c.Item().Height(10);

                            c.Item().Border(1).Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(25);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn();
                                    cd.RelativeColumn(1.5f);
                                });

                                void Header(string text)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(text).Bold();
                                }

                                Header("N°");
                                Header("Codigo Patrimonial");
                                Header("Denominacion");
                                Header("Marca");
                                Header("Modelo");
                                Header("Serie");
                                Header("Estado");
                                Header("Observaciones");

                                int index = 1;
                                foreach (var item in input.Bienes)
                                {
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(index++);
                                    t.Cell().Border(1).Padding(3).Text(item.B_CODACTIVO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_DESCRIP);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MARCA);
                                    t.Cell().Border(1).Padding(3).Text(item.B_MODE);
                                    t.Cell().Border(1).Padding(3).Text(item.B_SERIE);
                                    t.Cell().Border(1).Padding(3).AlignCenter().Text(item.B_ESTADO);
                                    t.Cell().Border(1).Padding(3).Text(item.B_OBS);
                                }

                            });

                            c.Item().Height(20);

                            c.Item().Row(r =>
                            {
                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter().Text("Responsable del Traslado");
                                });

                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter()
                                        .Text("Jefe de la Direccion u Oficina que autoriza el traslado");
                                });

                                r.RelativeItem().AlignCenter().Column(col =>
                                {
                                    col.Item().Height(25);
                                    col.Item().BorderTop(1);
                                    col.Item().AlignCenter().Text("Visto Bueno Patrimonio");
                                });
                            });
                        });
                    })
                ).GeneratePdf();

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

