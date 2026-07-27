using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class ChangeVadeSapParams
    {
        [SapName("IV_BUKRS ")]
        public string? IV_BUKRS { get; set; }
        [SapName("IV_BELNR ")]
        public string? IV_BELNR { get; set; }
        [SapName("IV_GJAHR ")]
        public string? IV_GJAHR { get; set; }
        [SapName("IV_DAY ")]
        public string? IV_DAY { get; set; }
    }
}
