
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
                        page.Margin(50);
                        page.Size(PageSizes.A4.Landscape());
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.
                            FontSize(10)
                            .FontFamily("Calibri")
                            .LineHeight(1.2f)
                            );

                        page.Header().Row(row =>
                        {
                            var logoBytes = File.ReadAllBytes("Data/logo.png");

                            row.ConstantItem(100)
                            .PaddingLeft(0.5f)
                            .Image(logoBytes)
                            .FitArea();

                            row.RelativeItem().AlignMiddle().Column(col =>
                            {
                                col.Item().Height(10);

                                col.Item().PaddingLeft(100).AlignMiddle().Text($"ASIGNACION EN USO DE BIENES MUEBLES PATRIMONIALES N° {nrodocu}-{DateTime.Now.Year}").Bold().FontSize(11);

                                col.Item().Height(10);

                            });
                                                
                        });

                        page.Content().Column(col =>
                        {
                            col.Item().Height(15);

                            col.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(9.6f);
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(3);
                                });

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Entidad:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text("Centro Nacional de Planeamiento Estarte - CEPLAN");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Fecha:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text($"{DateTime.Now:dd/MM/yyyy}");

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                    .AlignMiddle()
                                    .AlignLeft();

                            });

                            col.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {

                                    cd.ConstantColumn(120); 
                                    cd.RelativeColumn(5);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(3);
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(4);
                                });

                                t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text("1.- Datos del usuario ").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Nombres y Apellidos:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text($"{input.R_NOM}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_DNI);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Correo:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_CORREO);

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Condicion Contractual:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_CONTR);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Cargo:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_CARGO);

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Unidad Organica:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_UO);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Ubicacion Fisica:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_NOM_UBI_FISICA);

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Sede:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_SEDE);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Direccion de Usuario:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_DIREC);

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                        .AlignMiddle()
                                        .AlignLeft();

                            });

                            col.Item().Height(15);

                            col.Item().Table(t =>
                                    {
                                        t.ColumnsDefinition(cd =>
                                        {
                                            cd.ConstantColumn(25);   // N
                                            cd.ConstantColumn(100);  // Codigo
                                            cd.RelativeColumn(2);    // Denominacion
                                            cd.ConstantColumn(70);   // Marca
                                            cd.ConstantColumn(70);   // Modelo
                                            cd.ConstantColumn(70);   // Serie
                                            cd.ConstantColumn(70);   // Estado
                                            cd.RelativeColumn(4);    // Observaciones
                                        });

                                        t.Header(header =>
                                        {
                                            void HeaderCell(string text) =>
                                            header.Cell().Element(CellStyle).Padding(7).AlignCenter().Text(text).Bold();

                                            header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("N°");
                                            header.Cell().ColumnSpan(6).Element(CellStyle).Padding(7).AlignCenter().Text("Descripcion").Bold();
                                            header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("Observaciones");

                                            HeaderCell("Codigo Patrimonial");
                                            HeaderCell("Denominacion");
                                            HeaderCell("Marca");
                                            HeaderCell("Modelo");
                                            HeaderCell("Serie");
                                            HeaderCell("Estado");

                                            static IContainer CellStyle(IContainer container) =>
                                            container.DefaultTextStyle(x => x.SemiBold().FontSize(9))

                                            .Background(Colors.Grey.Lighten3)
                                            .Border(0.2f)
                                            .BorderColor(Colors.Grey.Lighten1);
                                        });

                                         int index = 1;

                                        foreach (var item in input.Bienes)
                                        {
                                            var bgColor = index % 2 == 0
                                                        ? Colors.Grey.Lighten5
                                                        : Colors.White;

                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(index);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_CODACTIVO);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_DESCRIP);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MARCA);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MODE);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_SERIE);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_ESTADO).Justify();
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_OBS).Justify();

                                            index ++;

                                        }

                                        static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                    .Border(0.2f)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Padding(8)
                                    .AlignMiddle()
                                    .AlignCenter();
                                    });

                            col.Item().Height(15);

                            col.Item().Row(row =>
                            {

                                row.RelativeItem()

                                    .BorderLeft(4).Background(Colors.Black)
                                    .Background(Colors.Grey.Lighten5)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .PaddingTop(5)
                                            .PaddingLeft(10)
                                        .Text("CONSIDERACIONES:")
                                        .Bold()
                                        .FontSize(9);

                                        col.Item()
                                        .PaddingLeft(10)
                                        .PaddingTop(5)
                                        .PaddingBottom(5)
                                        .Text("El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc. Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, es previamente comunicado al Especialista en Gestion Patrimonial y Almacen.")
                                        .FontSize(9);
                                    });
                            });

                            col.Item().Height(15);
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter().Text("Quien recibe").Bold();
                                    c.Item().Height(50);
                                    c.Item().Text("_______________________________________________________________");
                                    c.Item().AlignCenter().Text("Usuario").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter().Text("Quien entrega").Bold();
                                    c.Item().Height(50);
                                    c.Item().Text("_______________________________________________________________");
                                    c.Item().AlignCenter().Text("Patrimonio").Bold();
                                });
                            });

                        });

                        page.Footer()
                        .AlignRight()
                        .Padding(1)
                        .Text(x =>
                        {
                            x.Span("Página ").FontSize(9);
                            x.CurrentPageNumber().FontSize(9);
                            x.Span(" de ").FontSize(9);
                            x.TotalPages().FontSize(9);
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
                parameters.Add("@R_ID_UBI_FSA", input.R_ID_UBI_FISICA);
                parameters.Add("@R_NOM_UBI_FSA", input.R_NOM_UBI_FISICA);

                parameters.Add("@BIENES", tabla.AsTableValuedParameter("Patrimonio.T_BIENES"));
                parameters.Add("@S_CODSOLIC", input.S_CODSOLIC);
                parameters.Add("@S_FECHASIG", input.S_FECHASIG);
                parameters.Add("@S_USUARIO ", input.S_FECHADESPLA);

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
                        page.Margin(50);
                        page.Size(PageSizes.A4.Landscape());
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.
                            FontSize(10)
                            .FontFamily("Calibri")
                            .LineHeight(1.2f)
                            );

                        page.Header().Row(row =>
                        {
                            var logoBytes = File.ReadAllBytes("Data/logo.png");

                            row.ConstantItem(100)
                            .PaddingLeft(0.5f)
                            .Image(logoBytes)
                            .FitArea();

                            row.RelativeItem().AlignMiddle().Column(col =>
                            {
                                col.Item().Height(10);

                                col.Item().PaddingLeft(70).AlignMiddle().Text($"ASIGNACION EN USO DE BIENES MUEBLES PATRIMONIALES - TELETRABAJO N° {nrodocu}-{DateTime.Now.Year}").Bold().FontSize(11);

                                col.Item().Height(10);

                            });
                                                
                        });

                        page.Content().Column(col =>
                        {
                            col.Item().Height(15);

                            col.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(9.6f);
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(3);
                                });

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Entidad:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text("Centro Nacional de Planeamiento Estarte - CEPLAN");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Fecha:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text($"{DateTime.Now:dd/MM/yyyy}");

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                        .AlignMiddle()
                                        .AlignLeft();

                            });

                            col.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {

                                    cd.ConstantColumn(120); 
                                    cd.RelativeColumn(5);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(3);
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(4);
                                });

                                t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text("1.- Datos del usuario ").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Nombres y Apellidos:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_NOM);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_DNI);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Correo:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_CORREO);

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Condicion Contractual:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_CONTR);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Cargo:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_CARGO);

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Unidad Organica:").Bold();
                                t.Cell().ColumnSpan(5).Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_UO);

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Sede:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_SEDE);
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Direccion de Usuario:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_DIREC);

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                        .AlignMiddle()
                                        .AlignLeft();

                            });

                            col.Item().Height(15);

                            col.Item().Table(t =>
                                    {
                                        t.ColumnsDefinition(cd =>
                                        {
                                            cd.ConstantColumn(25);   // N
                                            cd.ConstantColumn(100);  // Codigo
                                            cd.RelativeColumn(2);    // Denominacion
                                            cd.ConstantColumn(70);   // Marca
                                            cd.ConstantColumn(70);   // Modelo
                                            cd.ConstantColumn(70);   // Serie
                                            cd.ConstantColumn(70);   // Estado
                                            cd.RelativeColumn(4);    // Observaciones
                                        });

                                        t.Header(header =>
                                        {
                                            void HeaderCell(string text) =>
                                            header.Cell().Element(CellStyle).Padding(7).AlignCenter().Text(text).Bold();

                                            header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("N°");
                                            header.Cell().ColumnSpan(6).Element(CellStyle).Padding(7).AlignCenter().Text("Descripcion").Bold();
                                            header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("Observaciones");

                                            HeaderCell("Codigo Patrimonial");
                                            HeaderCell("Denominacion");
                                            HeaderCell("Marca");
                                            HeaderCell("Modelo");
                                            HeaderCell("Serie");
                                            HeaderCell("Estado");

                                            static IContainer CellStyle(IContainer container) =>
                                            container.DefaultTextStyle(x => x.SemiBold().FontSize(9))

                                            .Background(Colors.Grey.Lighten3)
                                            .Border(0.2f)
                                            .BorderColor(Colors.Grey.Lighten1);
                                        });

                                        int index = 1;
                                        foreach (var item in input.Bienes )
                                        {
                                            var bgColor = index % 2 == 0
                                                        ? Colors.Grey.Lighten5
                                                        : Colors.White;

                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(index);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_CODACTIVO);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_DESCRIP);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MARCA);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MODE);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_SERIE);
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_ESTADO).Justify();
                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_OBS).Justify();

                                            index ++;

                                        }

                                        static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                                .Border(0.2f)
                                                .BorderColor(Colors.Grey.Lighten1)
                                                .Padding(8)
                                                .AlignMiddle()
                                                .AlignCenter();
                                    });

                            col.Item().Height(15);

                            col.Item().Row(row =>
                            {

                                row.RelativeItem()

                                    .BorderLeft(4).Background(Colors.Black)
                                    .Background(Colors.Grey.Lighten5)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .PaddingTop(5)
                                            .PaddingLeft(10)
                                        .Text("CONSIDERACIONES:")
                                        .Bold()
                                        .FontSize(9);

                                        col.Item()
                                        .PaddingLeft(10)
                                        .PaddingTop(5)
                                        .PaddingBottom(5)
                                        .Text("El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc. Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, es previamente comunicado al Especialista en Gestion Patrimonial y Almacen.")
                                        .FontSize(9);
                                    });
                            });

                            col.Item().Height(15);
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter().Text("Quien recibe").Bold();
                                    c.Item().Height(50);
                                    c.Item().Text("_______________________________________________________________");
                                    c.Item().AlignCenter().Text("Usuario").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter().Text("Quien entrega").Bold();
                                    c.Item().Height(50);
                                    c.Item().Text("_______________________________________________________________");
                                    c.Item().AlignCenter().Text("Patrimonio").Bold();
                                });
                            });

                        });

                        page.Footer()
                        .AlignRight()
                        .Padding(1)
                        .Text(x =>
                        {
                            x.Span("Página ").FontSize(9);
                            x.CurrentPageNumber().FontSize(9);
                            x.Span(" de ").FontSize(9);
                            x.TotalPages().FontSize(9);
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
                                        page.Margin(50);
                                        page.Size(PageSizes.A4.Landscape());
                                        page.PageColor(Colors.White);
                                        page.DefaultTextStyle(x => x.
                                            FontSize(10)
                                            .FontFamily("Calibri")
                                            .LineHeight(1.2f)
                                            );

                                        page.Header().Row(row =>
                                        {
                                            var logoBytes = File.ReadAllBytes("Data/logo.png");

                                            row.ConstantItem(100)
                                            .PaddingLeft(0.5f)
                                            .Image(logoBytes)
                                            .FitArea();

                                            row.RelativeItem().AlignMiddle().Column(col =>
                                            {
                                                col.Item().Height(10);

                                                col.Item().PaddingLeft(120).AlignMiddle().Text($"DEVOLUCION DE BIENES MUEBLES PATRIMONIALES N° {nrodocu}-{DateTime.Now.Year}").Bold().FontSize(11);

                                                col.Item().Height(10);

                                            });
                                                                
                                        });

                                        page.Content().Column(col =>
                                        {
                                            col.Item().Height(15);

                                            col.Item().Table(t =>
                                            {
                                                t.ColumnsDefinition(cd =>
                                                {
                                                    cd.ConstantColumn(50);
                                                    cd.RelativeColumn(9.6f);
                                                    cd.ConstantColumn(50);
                                                    cd.RelativeColumn(3);
                                                });

                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Entidad:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text("Centro Nacional de Planeamiento Estarte - CEPLAN");
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Fecha:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text($"{DateTime.Now:dd/MM/yyyy}");

                                                static IContainer CellData(IContainer container) =>
                                                        container.DefaultTextStyle(x => x.FontSize(9))
                                                        .Padding(7)
                                                    .AlignMiddle()
                                                    .AlignLeft();

                                            });

                                            col.Item().Table(t =>
                                            {
                                                t.ColumnsDefinition(cd =>
                                                {

                                                    cd.ConstantColumn(120); 
                                                    cd.RelativeColumn(5);
                                                    cd.RelativeColumn(3);
                                                    cd.RelativeColumn(3);
                                                    cd.ConstantColumn(50);
                                                    cd.RelativeColumn(4);
                                                });

                                                t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text("1.- Datos del usuario ").Bold();

                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Nombres y Apellidos:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_NOM);
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_DNI);
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Correo:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_CORREO);

                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Condicion Contractual:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_CONTR);
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Cargo:").Bold();
                                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_CARGO);

                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Unidad Organica:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_UO);

                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Ubicacion Fisica:").Bold();
                                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_NOM_UBI_FISICA);

                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Sede:").Bold();
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_SEDE);
                                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Direccion de Usuario:").Bold();
                                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten1).Border(0.5f).Element(CellData).Text(input.R_DIREC);

                                                static IContainer CellData(IContainer container) =>
                                                        container.DefaultTextStyle(x => x.FontSize(9))
                                                        .Padding(7)
                                                    .AlignMiddle()
                                                    .AlignLeft();

                                            });

                                            col.Item().Height(15);

                                            col.Item().Table(t =>
                                                    {
                                                        t.ColumnsDefinition(cd =>
                                                        {
                                                            cd.ConstantColumn(25);   // N
                                                            cd.ConstantColumn(100);  // Codigo
                                                            cd.RelativeColumn(2);    // Denominacion
                                                            cd.ConstantColumn(70);   // Marca
                                                            cd.ConstantColumn(70);   // Modelo
                                                            cd.ConstantColumn(70);   // Serie
                                                            cd.ConstantColumn(70);   // Estado
                                                            cd.RelativeColumn(4);    // Observaciones
                                                        });

                                                        t.Header(header =>
                                                        {
                                                            void HeaderCell(string text) =>
                                                            header.Cell().Element(CellStyle).Padding(7).AlignCenter().Text(text).Bold();

                                                            header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("N°");
                                                            header.Cell().ColumnSpan(6).Element(CellStyle).Padding(7).AlignCenter().Text("Descripcion").Bold();
                                                            header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("Observaciones");

                                                            HeaderCell("Codigo Patrimonial");
                                                            HeaderCell("Denominacion");
                                                            HeaderCell("Marca");
                                                            HeaderCell("Modelo");
                                                            HeaderCell("Serie");
                                                            HeaderCell("Estado");

                                                            static IContainer CellStyle(IContainer container) =>
                                                            container.DefaultTextStyle(x => x.SemiBold().FontSize(9))

                                                            .Background(Colors.Grey.Lighten3)
                                                            .Border(0.2f)
                                                            .BorderColor(Colors.Grey.Lighten1);
                                                        });

                                                        int index = 1;
                                                        foreach (var item in ventasEjemplo)
                                                        {
                                                            var bgColor = index % 2 == 0
                                                                        ? Colors.Grey.Lighten5
                                                                        : Colors.White;

                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(index);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_CODACTIVO);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_DESCRIP);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MARCA);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MODE);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_SERIE);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_ESTADO).Justify();
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_OBS).Justify();

                                                            index++;

                                                        }

                                                        static IContainer CellData(IContainer container) =>
                                                        container.DefaultTextStyle(x => x.FontSize(9))
                                                    .Border(0.2f)
                                                    .BorderColor(Colors.Grey.Lighten1)
                                                    .Padding(8)
                                                    .AlignMiddle()
                                                    .AlignCenter();
                                                    });

                                            col.Item().Height(15);

                                            col.Item().Row(row =>
                                            {

                                                row.RelativeItem()

                                                    .BorderLeft(4).Background(Colors.Black)
                                                    .Background(Colors.Grey.Lighten5)
                                                    .Column(col =>
                                                    {
                                                        col.Item()
                                                            .PaddingTop(5)
                                                            .PaddingLeft(10)
                                                        .Text("CONSIDERACIONES:")
                                                        .Bold()
                                                        .FontSize(9);

                                                        col.Item()
                                                        .PaddingLeft(10)
                                                        .PaddingTop(5)
                                                        .PaddingBottom(5)
                                                        .Text("El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc. Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, es previamente comunicado al Especialista en Gestion Patrimonial y Almacen.")
                                                        .FontSize(9);
                                                    });
                                            });

                                            col.Item().Height(15);
                                            col.Item().Row(row =>
                                            {
                                                row.RelativeItem().AlignCenter().Column(c =>
                                                {
                                                    c.Item().AlignCenter().Text("Quien recibe").Bold();
                                                    c.Item().Height(50);
                                                    c.Item().Text("_______________________________________________________________");
                                                    c.Item().AlignCenter().Text("Usuario").Bold();
                                                });

                                                row.RelativeItem().AlignCenter().Column(c =>
                                                {
                                                    c.Item().AlignCenter().Text("Quien entrega").Bold();
                                                    c.Item().Height(50);
                                                    c.Item().Text("_______________________________________________________________");
                                                    c.Item().AlignCenter().Text("Patrimonio").Bold();
                                                });
                                            });

                                        });

                                        page.Footer()
                                        .AlignRight()
                                        .Padding(1)
                                        .Text(x =>
                                        {
                                            x.Span("Página ").FontSize(9);
                                            x.CurrentPageNumber().FontSize(9);
                                            x.Span(" de ").FontSize(9);
                                            x.TotalPages().FontSize(9);
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
                parameters.Add("@R_ID_UBI_FSA", input.R_ID_UBI_FISICA);
                parameters.Add("@R_NOM_UBI_FSA", input.R_NOM_UBI_FISICA);

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

                byte[] pdf = QuestPDF.Fluent.Document.Create( container =>
                {
                    
                   container.Page(page =>
                        {
                            page.Margin(50);
                            page.Size(PageSizes.A4.Landscape());
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.
                                FontSize(10)
                                .FontFamily("Calibri")
                                .LineHeight(1.2f)
                                );

                            page.Header().Row(row =>
                            {
                                var logoBytes = File.ReadAllBytes("Data/logo.png");

                                row.ConstantItem(100)
                                .PaddingLeft(0.5f)
                                .Image(logoBytes)
                                .FitArea();

                                row.RelativeItem().AlignMiddle().Column(col =>
                                {
                                    col.Item().Height(10);

                                    col.Item().PaddingLeft(120).AlignMiddle().Text($"DESPLAZAMIENTO DE BIENES MUEBLES PATRIMONIALES N° {nrodocu}-{DateTime.Now.Year}").Bold().FontSize(11);
                                   

                                    col.Item().Height(10);

                                });
                                                    
                            });

                            page.Content().Column(col =>
                            {
                                col.Item().Height(15);

                                col.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(cd =>
                                    {
                                        cd.ConstantColumn(50);
                                        cd.RelativeColumn(9.6f);
                                        cd.ConstantColumn(50);
                                        cd.RelativeColumn(3);
                                    });

                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Entidad:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text("Centro Nacional de Planeamiento Estarte - CEPLAN");
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Fecha:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text($"{DateTime.Now:dd/MM/yyyy}");

                                    static IContainer CellData(IContainer container) =>
                                            container.DefaultTextStyle(x => x.FontSize(9))
                                            .Padding(7)
                                        .AlignMiddle()
                                        .AlignLeft();

                                });

                                col.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(cd =>
                                    {

                                        cd.ConstantColumn(120); 
                                        cd.RelativeColumn(5);
                                        cd.RelativeColumn(3);
                                        cd.RelativeColumn(3);
                                        cd.ConstantColumn(50);
                                        cd.RelativeColumn(4);
                                    });

                                    t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text("I.- USUARIO QUE TRANSFIERE").Bold();

                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Nombres y Apellidos:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.A_NOM);
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.A_DNI);
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Correo:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.A_CORREO);

                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Unidad Organica:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.A_UO);
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Sede:").Bold();
                                    t.Cell().ColumnSpan(3).Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.A_SEDE);

                                    t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text("II.- USUARIO QUE RECEPCIONA").Bold();

                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Nombres y Apellidos:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_NOM);
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_DNI);
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Correo:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_CORREO);

                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Unidad Organica:").Bold();
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_UO);
                                    t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Element(CellData).Text("Sede:").Bold();
                                    t.Cell().ColumnSpan(3).Border(0.5f).BorderColor(Colors.Grey.Lighten1).Element(CellData).Text(input.R_SEDE);


                                    static IContainer CellData(IContainer container) =>
                                            container.DefaultTextStyle(x => x.FontSize(9))
                                            .Padding(7)
                                        .AlignMiddle()
                                        .AlignLeft();

                                });

                                col.Item().Height(15);

                                col.Item().Table(t =>
                                        {
                                            t.ColumnsDefinition(cd =>
                                            {
                                                cd.ConstantColumn(25);   // N
                                                cd.ConstantColumn(100);  // Codigo
                                                cd.RelativeColumn(2);    // Denominacion
                                                cd.ConstantColumn(70);   // Marca
                                                cd.ConstantColumn(70);   // Modelo
                                                cd.ConstantColumn(70);   // Serie
                                                cd.ConstantColumn(70);   // Estado
                                                cd.RelativeColumn(4);    // Observaciones
                                            });

                                            t.Header(header =>
                                            {
                                                void HeaderCell(string text) =>
                                                header.Cell().Element(CellStyle).Padding(7).AlignCenter().Text(text).Bold();

                                                header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("N°");
                                                header.Cell().ColumnSpan(6).Element(CellStyle).Padding(7).AlignCenter().Text("Descripcion").Bold();
                                                header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("Observaciones");

                                                HeaderCell("Codigo Patrimonial");
                                                HeaderCell("Denominacion");
                                                HeaderCell("Marca");
                                                HeaderCell("Modelo");
                                                HeaderCell("Serie");
                                                HeaderCell("Estado");

                                                static IContainer CellStyle(IContainer container) =>
                                                container.DefaultTextStyle(x => x.SemiBold().FontSize(9))

                                                .Background(Colors.Grey.Lighten3)
                                                .Border(0.2f)
                                                .BorderColor(Colors.Grey.Lighten1);
                                            });

                                            int index = 1;
                                                        foreach (var item in ventasEjemplo)
                                                        {
                                                            var bgColor = index % 2 == 0
                                                                        ? Colors.Grey.Lighten5
                                                                        : Colors.White;

                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(index);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_CODACTIVO);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_DESCRIP);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MARCA);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MODE);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_SERIE);
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_ESTADO).Justify();
                                                            t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_OBS).Justify();

                                                            index++;

                                                        }
                                            static IContainer CellData(IContainer container) =>
                                            container.DefaultTextStyle(x => x.FontSize(9))
                                        .Border(0.2f)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .Padding(8)
                                        .AlignMiddle()
                                        .AlignCenter();
                                        });

                                col.Item().Height(15);

                                col.Item().Row(row =>
                                {

                                    row.RelativeItem()

                                        .BorderLeft(4).Background(Colors.Black)
                                        .Background(Colors.Grey.Lighten5)
                                        .Column(col =>
                                        {
                                            col.Item()
                                                .PaddingTop(5)
                                                .PaddingLeft(10)
                                            .Text("CONSIDERACIONES:")
                                            .Bold()
                                            .FontSize(9);

                                            col.Item()
                                            .PaddingLeft(10)
                                            .PaddingTop(5)
                                            .PaddingBottom(5)
                                            .Text("El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc. Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, es previamente comunicado al Especialista en Gestion Patrimonial y Almacen.")
                                            .FontSize(9);
                                        });
                                });

                                col.Item().Height(15);
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter().Text("Quien entrega:").Bold();
                                    c.Item().Height(50);
                                    c.Item().Text("_________________________________________");
                                    c.Item().AlignCenter().Text("Usuario").Bold();
                                });

                                    row.RelativeItem().AlignCenter().Column(c =>
                                    {
                                        c.Item().AlignCenter().Text("Quien recibe:").Bold();
                                        c.Item().Height(50);
                                        c.Item().Text("________________________________________");
                                        c.Item().AlignCenter().Text("Usuario").Bold();
                                    });

                                    row.RelativeItem().AlignCenter().Column(c =>
                                    {
                                        c.Item().AlignCenter().Text("Visto Bueno:").Bold();
                                        c.Item().Height(50);
                                        c.Item().Text("_________________________________________");
                                        c.Item().AlignCenter().Text("Patrimonio").Bold();
                                    });
                                });

                            });

                            page.Footer()
                            .AlignRight()
                            .Padding(1)
                            .Text(x =>
                            {
                                x.Span("Página ").FontSize(9);
                                x.CurrentPageNumber().FontSize(9);
                                x.Span(" de ").FontSize(9);
                                x.TotalPages().FontSize(9);
                            });
                        });
                    }
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
                        p.Margin(50);
                        p.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                        p.PageColor(Colors.White);
                        p.DefaultTextStyle(x => x.
                            FontSize(9)
                            .FontFamily("Calibri")
                            .LineHeight(1.2f)
                            );

                        p.Header().Row(row =>
                        {
                            var logoBytes = File.ReadAllBytes("Data/logo.png");

                            row.RelativeItem().AlignMiddle().Column(col =>
                            {
                                col.Item().Height(10);
                                col.Item().AlignCenter().Text($" ORDEN DE SALIDA Y REINGRESO DE BIENES MUEBLES PATRIMONIALES N° {nrodocu}-{DateTime.Now.Year}").Bold().FontSize(11);
                                col.Item().Height(10);
                            });

                        });

                        p.Content().Column(c =>
                        {

                            c.Item().Height(15);

                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(9.6f);
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(3);
                                });

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Entidad:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text("Centro Nacional de Planeamiento Estarte - CEPLAN");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Fecha:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{DateTime.Now:dd/MM/yyyy}");

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                    .AlignMiddle()
                                    .AlignLeft();

                            });


                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(2);

                                });

                                int tipoSalida = int.TryParse(input.S_TIPO_SALIDA, out int value) ? value : 0;

                                string motivo = input.S_MTV_SALIDA;

                                t.Cell().ColumnSpan(4).Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Element(CellData).AlignCenter().Text("Tipo").Bold();
                                t.Cell().ColumnSpan(6).Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Element(CellData).AlignCenter().Text("Motivo").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Salida:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(tipoSalida == 1 ? "X" : " ").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Reingreso:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(tipoSalida == 2 ? "X" : " ").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Mantenimiento:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(motivo == "1" ? "X" : " ").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Comision de servicios:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(motivo == "2" ? "X" : " ").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Capacitacion/Evento:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(motivo == "3" ? "X" : " ").Bold();

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                        .AlignMiddle()
                                        .AlignLeft();

                            });

                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {

                                    cd.ConstantColumn(100);
                                    cd.RelativeColumn(5);
                                    cd.ConstantColumn(55);
                                    cd.RelativeColumn(5);
                                    cd.ConstantColumn(55);
                                    cd.RelativeColumn(4);
                                });

                                t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Border(0.5f).Element(CellData).Text("I.- DATOS DEL RESPONSABLE DEL TRASLADO").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Nombres y Apellidos:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_NOM}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_DNI}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Correo:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_CORREO}");

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Unidad Organica:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_UO}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("sede:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text($"{input.A_SEDE}");

                                t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Border(0.5f).Element(CellData).Text("II.- DATOS DEL DESTINO").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Representante usuario:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.R_NOM}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni/Ruc:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.R_DNI}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Direccion:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.R_DIREC}");

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Proveedor/Unidad Organica:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"input.R_UO");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("sede:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text($"input.R_SEDE");

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                    .AlignMiddle()
                                    .AlignLeft();

                            });

                            c.Item().Height(15);

                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(25);   // N
                                    cd.ConstantColumn(100);   // Codigo
                                    cd.RelativeColumn(2);    // Denominacion
                                    cd.ConstantColumn(70);     // Marca
                                    cd.ConstantColumn(70);     // Modelo
                                    cd.ConstantColumn(70);     // Serie
                                    cd.ConstantColumn(70);   // Estado
                                    cd.RelativeColumn(4);     // Observaciones
                                });

                                t.Header(header =>
                                {
                                    void HeaderCell(string text) =>
                                        header.Cell().Element(CellStyle).Padding(7).AlignCenter().Text(text).Bold();

                                    header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("N°");
                                    header.Cell().ColumnSpan(6).Element(CellStyle).Padding(7).AlignCenter().Text("Descripcion").Bold();
                                    header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("Observaciones");

                                    HeaderCell("Codigo Patrimonial");
                                    HeaderCell("Denominacion");
                                    HeaderCell("Marca");
                                    HeaderCell("Modelo");
                                    HeaderCell("Serie");
                                    HeaderCell("Estado");

                                    static IContainer CellStyle(IContainer container) =>
                                    container.DefaultTextStyle(x => x.SemiBold().FontSize(9))

                                    .Background(Colors.Grey.Lighten4)
                                    .Border(0.2f)
                                    .BorderColor(Colors.Grey.Lighten3);
                                });

                                int index = 1;

                                foreach (var v in input.Bienes)
                                {
                                    var bgColor = index % 2 == 0
                                                ? Colors.Grey.Lighten5
                                                : Colors.White;

                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(index);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_CODACTIVO);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_DESCRIP);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MARCA);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MODE);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_SERIE);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_ESTADO).Justify();
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_OBS).Justify();

                                    index++;

                                }

                                static IContainer CellData(IContainer container) =>
                                container.DefaultTextStyle(x => x.FontSize(9))
                            .Border(0.2f)
                            .BorderColor(Colors.Grey.Lighten3)
                            .AlignMiddle()
                            .AlignCenter()
                            .Padding(5);
                            });

                            c.Item().Height(15);

                            c.Item().Row(row =>
                            {

                                row.RelativeItem()

                                    .BorderLeft(4).Background(Colors.Black)
                                    .Background(Colors.Grey.Lighten5)
                                    .Column(col =>
                                    {
                                        c.Item()
                                            .PaddingTop(5)
                                            .PaddingLeft(10)
                                           .Text("CONSIDERACIONES:")
                                           .Bold()
                                           .FontSize(9);

                                        c.Item()
                                           .PaddingLeft(10)
                                           .PaddingTop(5)
                                           .PaddingBottom(5)
                                           .Text("El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc. Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, es previamente comunicado al Especialista en Gestion Patrimonial y Almacen.")
                                           .FontSize(9);
                                    });
                            });

                            c.Item().Height(15);
                            c.Item().Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Column(c =>
                                {

                                    c.Item().Height(60);
                                    c.Item().Text("________________________________________");
                                    c.Item().AlignCenter().Text("Responsable del Traslado").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {

                                    c.Item().Height(60);
                                    c.Item().Text("________________________________________");
                                    c.Item().AlignCenter().Text("Jefe de la Oficina que Traslada el bien").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {

                                    c.Item().Height(60);
                                    c.Item().Text("________________________________________");
                                    c.Item().AlignCenter().Text("Visto bueno de Patrimonio").Bold();
                                });

                            });

                        });//columna

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
                        p.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                        p.Margin(50);
                        p.PageColor(Colors.White);
                        p.DefaultTextStyle(x => x.
                    FontSize(9)
                    .FontFamily("Calibri")
                    .LineHeight(1.2f)
                    );


                        p.Header().Row(row =>
                            {
                                var logoBytes = File.ReadAllBytes("Data/logo.png");

                                row.RelativeItem().AlignMiddle().Column(col =>
                                {
                                    col.Item().Height(10);
                                    col.Item().AlignCenter().Text($" ORDEN DE SALIDA Y REINGRESO DE BIENES MUEBLES PATRIMONIALES N° {nrodocu}-{DateTime.Now.Year}").Bold().FontSize(11);
                                    col.Item().Height(10);
                                });

                            });

                        p.Content().Column(c =>
                        {

                            c.Item().Height(15);

                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(9.6f);
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(3);
                                });

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Entidad:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text("Centro Nacional de Planeamiento Estarte - CEPLAN");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Fecha:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{DateTime.Now:dd/MM/yyyy}");

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                    .AlignMiddle()
                                    .AlignLeft();

                            });


                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(50);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(2);
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(2);

                                });

                                int tipoSalida = int.TryParse(input.S_TIPO_SALIDA, out int value) ? value : 0;

                                string motivo = input.S_MTV_SALIDA;

                                t.Cell().ColumnSpan(4).Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Element(CellData).AlignCenter().Text("Tipo").Bold();
                                t.Cell().ColumnSpan(6).Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Element(CellData).AlignCenter().Text("Motivo").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Salida:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(tipoSalida == 1 ? "X" : " ").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Reingreso:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(tipoSalida == 2 ? "X" : " ").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Mantenimiento:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(motivo == "1" ? "X" : " ").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Comision de servicios:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(motivo == "2" ? "X" : " ").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Capacitacion/Evento:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).AlignCenter().Text(motivo == "3" ? "X" : " ").Bold();

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                        .AlignMiddle()
                                        .AlignLeft();

                            });

                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {

                                    cd.ConstantColumn(100);
                                    cd.RelativeColumn(5);
                                    cd.ConstantColumn(55);
                                    cd.RelativeColumn(5);
                                    cd.ConstantColumn(55);
                                    cd.RelativeColumn(4);
                                });

                                t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Border(0.5f).Element(CellData).Text("I.- DATOS DEL RESPONSABLE DEL TRASLADO").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Nombres y Apellidos:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_NOM}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_DNI}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Correo:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_CORREO}");

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Unidad Organica:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.A_UO}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("sede:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text($"{input.A_SEDE}");

                                t.Cell().ColumnSpan(6).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten4).Border(0.5f).Element(CellData).Text("II.- DATOS DEL DESTINO").Bold();

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Representante usuario:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.R_NOM}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Dni/Ruc:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.R_DNI}");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Direccion:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"{input.R_DIREC}");

                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("Proveedor/Unidad Organica:").Bold();
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Element(CellData).Text($"input.R_UO");
                                t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten3).Background(Colors.Grey.Lighten5).Element(CellData).Text("sede:").Bold();
                                t.Cell().ColumnSpan(3).BorderColor(Colors.Grey.Lighten3).Border(0.5f).Element(CellData).Text($"input.R_SEDE");

                                static IContainer CellData(IContainer container) =>
                                        container.DefaultTextStyle(x => x.FontSize(9))
                                        .Padding(7)
                                    .AlignMiddle()
                                    .AlignLeft();

                            });

                            c.Item().Height(15);

                            c.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(25);   // N
                                    cd.ConstantColumn(100);   // Codigo
                                    cd.RelativeColumn(2);    // Denominacion
                                    cd.ConstantColumn(70);     // Marca
                                    cd.ConstantColumn(70);     // Modelo
                                    cd.ConstantColumn(70);     // Serie
                                    cd.ConstantColumn(70);   // Estado
                                    cd.RelativeColumn(4);     // Observaciones
                                });

                                t.Header(header =>
                                {
                                    void HeaderCell(string text) =>
                                        header.Cell().Element(CellStyle).Padding(7).AlignCenter().Text(text).Bold();

                                    header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("N°");
                                    header.Cell().ColumnSpan(6).Element(CellStyle).Padding(7).AlignCenter().Text("Descripcion").Bold();
                                    header.Cell().RowSpan(2).Element(CellStyle).Padding(4).AlignMiddle().AlignCenter().Text("Observaciones");

                                    HeaderCell("Codigo Patrimonial");
                                    HeaderCell("Denominacion");
                                    HeaderCell("Marca");
                                    HeaderCell("Modelo");
                                    HeaderCell("Serie");
                                    HeaderCell("Estado");

                                    static IContainer CellStyle(IContainer container) =>
                                    container.DefaultTextStyle(x => x.SemiBold().FontSize(9))

                                    .Background(Colors.Grey.Lighten4)
                                    .Border(0.2f)
                                    .BorderColor(Colors.Grey.Lighten3);
                                });

                                int index = 1;

                                foreach (var v in input.Bienes)
                                {
                                    var bgColor = index % 2 == 0
                                                ? Colors.Grey.Lighten5
                                                : Colors.White;

                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(index);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_CODACTIVO);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_DESCRIP);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MARCA);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_MODE);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_SERIE);
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_ESTADO).Justify();
                                    t.Cell().Background(bgColor).Element(CellData).ShowEntire().Text(item.B_OBS).Justify();

                                    index++;

                                }

                                static IContainer CellData(IContainer container) =>
                                container.DefaultTextStyle(x => x.FontSize(9))
                            .Border(0.2f)
                            .BorderColor(Colors.Grey.Lighten3)
                            .AlignMiddle()
                            .AlignCenter()
                            .Padding(5);
                            });

                            c.Item().Height(15);

                            c.Item().Row(row =>
                            {

                                row.RelativeItem()

                                    .BorderLeft(4).Background(Colors.Black)
                                    .Background(Colors.Grey.Lighten5)
                                    .Column(col =>
                                    {
                                        c.Item()
                                            .PaddingTop(5)
                                            .PaddingLeft(10)
                                           .Text("CONSIDERACIONES:")
                                           .Bold()
                                           .FontSize(9);

                                        c.Item()
                                           .PaddingLeft(10)
                                           .PaddingTop(5)
                                           .PaddingBottom(5)
                                           .Text("El usuario es responsable de la permanencia y conservacion de cada uno de los bienes descritos, debiendo tomar las precauciones del caso para evitar sustracciones, deterioros, etc. Cualquier necesidad de traslado del bien mueble patrimonial dentro o fuera del local de la Entidad, es previamente comunicado al Especialista en Gestion Patrimonial y Almacen.")
                                           .FontSize(9);
                                    });
                            });

                            c.Item().Height(15);
                            c.Item().Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Column(c =>
                                {

                                    c.Item().Height(60);
                                    c.Item().Text("________________________________________");
                                    c.Item().AlignCenter().Text("Responsable del Traslado").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {

                                    c.Item().Height(60);
                                    c.Item().Text("________________________________________");
                                    c.Item().AlignCenter().Text("Jefe de la Oficina que Traslada el bien").Bold();
                                });

                                row.RelativeItem().AlignCenter().Column(c =>
                                {

                                    c.Item().Height(60);
                                    c.Item().Text("________________________________________");
                                    c.Item().AlignCenter().Text("Visto bueno de Patrimonio").Bold();
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

