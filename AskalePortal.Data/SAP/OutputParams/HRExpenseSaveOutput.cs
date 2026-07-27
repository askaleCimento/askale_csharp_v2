using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class HRExpenseSaveOutput
    {
        [SapName("MESSTAB")]
        public Bdcmsgcool[]? message { get; set; }

    }
}
