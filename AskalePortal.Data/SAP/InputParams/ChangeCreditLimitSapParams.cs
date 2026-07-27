using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class ChangeCreditLimitSapParams
    {
        [SapName("IV_KUNNR")]
        public string? IV_KUNNR { get; set; }
        [SapName("IV_ADD_LIMIT")]
        public string? IV_ADD_LIMIT { get; set; }
    }
}
