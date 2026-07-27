using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{

   

    public class HRExpenseInputToSave
    {
        [SapName("TARIH")]
        public string? TARIH { get; set; }

        [SapName("TEXT1")]
        public string? TEXT1 { get; set; }

        [SapName("TEXT2")]
        public string? TEXT2 { get; set; }

        [SapName("SIRKET")]
        public string? SIRKET { get; set; }

        [SapName("PERNO")]
        public string? PERNO { get; set; }

        [SapName("IM_TABLE")]
        public HRExpenseInput2[]? hRExpenseInput2 { get; set; }
    }

    public class HRExpenseInput2
    {
        [SapName("SGTXT")]
        public string? SGTXT { get; set; }

        [SapName("WRBTR")]
        public string? WRBTR { get; set; }

        [SapName("MWSKZ")]
        public string? MWSKZ { get; set; }

        [SapName("FATURAMI")]
        public string? FATURAMI { get; set; }

        [SapName("UCAKMI")]
        public string? UCAKMI { get; set; }
      

    }
}
