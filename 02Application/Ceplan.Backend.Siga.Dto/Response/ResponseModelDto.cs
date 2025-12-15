using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Application.Dto.Response
{
    //tipos de respuesta que puede dar
    public class ResponseModelDto
    {
        public bool bSuccess { get; set; }
        public string? sMessage { get; set; }
    }
    public class ResponseModelDto<T>: ResponseModelDto
    {
        public T oData { get; set; }
    }

    public class ResponsePagedModelDto<T>
    {
        public bool bSuccess { get; set; }
        public string? sMessage { get; set; }
        public PagedResponseDto<T> oData { get; set; }
    }

    public class PagedResponseDto<T>  //no entiendo esta parte
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalNumberOfPages { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public List<T> Results { get; set; }
    }
}
