using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class AnnualLeaveSapInputParams
    {
        [SapName("LV_PERNR")]
        public required string perNo { get; set; }

        [SapName("P_GJAHR")]
        public required int gjahr { get; set; }

    }
}
