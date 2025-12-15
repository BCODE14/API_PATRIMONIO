namespace Ceplan.Backend.Siga.Transversal.Configuration
{
    public class AppSettings
    {
        public ParametrosGetToken ParametrosGetToken { get; set; }
        public ServiciosFirmaPeruApi ServiciosFirmaPeruApi { get; set; }
        public Urls Urls { get; set; }
        public Dlls Dlls { get; set; }
    }

    public class ParametrosGetToken
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class ServiciosFirmaPeruApi
    {
        public string urlGetToken { get; set; }

    }

    public class Urls
    {
        public string UrlSubirDoc { get; set; }
        public string UrlAgenteAutomatizado { get; set; }
        public string urlPlantillaRefirma { get; set; }

    }

    public class Dlls
    {
        public string SevenZipDll { get; set; }

    }
}
