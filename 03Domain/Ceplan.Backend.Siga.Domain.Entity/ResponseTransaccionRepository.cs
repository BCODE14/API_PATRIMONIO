using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Entity
{
    public class ResponseTransaccionRepository<T>
    {
        public int STATUS_CODE { get; set; }
        public T? VALUE { get; set; }
        public string? ERROR_MESSAGE { get; set; }
        public string? ADICIONAL_DATA { get; set; }

    }
}
