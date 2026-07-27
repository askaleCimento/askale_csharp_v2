using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class ChangeCustomerDocumentDateInput
    {
        [SapName("IV_BUKRS")]
        public required string bukrs { get; set; }
        [SapName("IV_BELNR")]
        public required string belnr { get; set; }
        [SapName("IV_GJAHR")]
        public required string gjahr { get; set; }
        [SapName("IV_DAY")]
        public required int ivday { get; set; }
      
    }
}
