using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class CustomerNoParams
    {
        [SapName("IV_KUNNR")]
        public string? kunnr { get; set; }

        [SapName("IV_BUKRS")]
        public string? bukrs { get; set; }
    }
}
