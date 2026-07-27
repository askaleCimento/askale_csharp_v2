using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class CustomerCreditLimitIncreaseReturn
    {
        [SapName("EV_RETURN")]
        public string? evreturn { get; set; }

        [SapName("EV_MESSAGE")]
        public string? evmessage { get; set; }
    }
}
