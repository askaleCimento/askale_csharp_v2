using AskalePortal.Data.SAP.OutputParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class ActiveProcessInvoiceSaveDto
    {
        public List<CustomerDocumentDto>? listCustomerDocumentSap { get; set; }
        public int activeProcessId { get; set; }
    }
}
