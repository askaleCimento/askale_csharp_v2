using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class ChecksParams
    {
        [SapName("IV_BUKRS")]
        public string? bukrs { get; set; }
        [SapName("IV_KUNNR")]
        public string? kunnr { get; set; }
        [SapName("IV_PORTFO")]
        public string? portfo { get; set; }
    }
}
