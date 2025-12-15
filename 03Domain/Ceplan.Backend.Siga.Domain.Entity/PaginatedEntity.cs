using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceplan.Backend.Siga.Domain.Entity
{
    public class PaginatedEntity
    {
        // INPUT
        public int? PAGE_NUMBER { get; set; }
        public int? PAGE_SIZE { get; set; }
        public string? SORT_COLUMN_NAME { get; set; }
        public string? SORT_ORDER { get; set; }
        public string? FILTER_VALUE  { get; set; }

        // OUTPUT
        public int? TOTAL_ROWS { get; set; }
        public int? ROW_NUMBER { get; set; }

    // BOTH
    public int? STATUS_CODE { get; set; }

    }
}
