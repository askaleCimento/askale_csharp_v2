using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.Models;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class CustomerSikayetOutput
    {
        [SapName("OUTPUT")]
        public CustomerSikayetList[]? customerSikayetList { get; set; }
    }

    public class CustomerSikayetList
    {
        [SapName("KUNNR")]
        public string? kunnr { get; set; }
        [SapName("NAME1")]
        public string? name1 { get; set; }

    }
}
