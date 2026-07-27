using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class CustomerCreditIncreaseLimitInput
    {
        [SapName("IV_KUNNR")]
        public string? kunnr { get; set; }

        [SapName("IV_ADD_LIMIT")]
        public decimal? limit { get; set; }
    }
}
