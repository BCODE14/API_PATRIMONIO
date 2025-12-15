namespace Ceplan.Backend.Siga.Transversal
{
    public class BaseResponseApi
    {
        public bool success { get; set; } = false;
        public string? mensaje { get; set; }
        public string data { get; set; } = default!;
    }
    public class BaseResponse
    {
        public bool success { get; set; } = false;
        public string? message { get; set; }
    }

    public class BaseResponseGeneric<T> : BaseResponse
    {
        public T data { get; set; } = default!;
    }
    public class BaseResponsePagination<T> : BaseResponse
    {
        public ICollection<T>? data { get; set; }
        public int paginaIndex { get; set; }
        public int registrosxPagina { get; set; }
    }
}