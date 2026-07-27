using AskalePortal.Data.SAP.Models;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class CustomerOutput
    {
        [SapName("OUTPUT")]
        public ListCustomer[]? customer { get; set; }

    }
    public class ListCustomer
    {
        [SapName("ADRNR")]
        public string? ADRNR { get; set; }

        [SapName("NAME1")]
        public string? NAME1 { get; set; }

        [SapName("DUEFL")]
        public string? DUEFL { get; set; }

        [SapName("ERDAT")]
        public DateTime? ERDAT { get; set; }

        [SapName("ERNAM")]
        public string? ERNAM { get; set; }

        [SapName("KTOKD")]
        public string? KTOKD { get; set; }

        [SapName("KUNNR")]
        public string? KUNNR { get; set; }

        [SapName("LAND1")]
        public string? LAND1 { get; set; }

        [SapName("LIFNR")]
        public string? LIFNR { get; set; }

        [SapName("NAME2")]
        public string? NAME2 { get; set; }

        [SapName("ORT01")]
        public string? ORT01 { get; set; }

        [SapName("PSTLZ")]
        public string? PSTLZ { get; set; }

        [SapName("REGIO")]
        public string? REGIO { get; set; }

        [SapName("SORTL")]
        public string? SORTL { get; set; }

        [SapName("SPERR")]
        public string? SPERR { get; set; }

        [SapName("SPRAS")]
        public string? SPRAS { get; set; }

        [SapName("STCD1")]
        public string? STCD1 { get; set; }

        [SapName("STCD2")]
        public string? STCD2 { get; set; }

        [SapName("STRAS")]
        public string? STRAS { get; set; }

        [SapName("TELF1")]
        public string? TELF1 { get; set; }

        [SapName("TELF2")]
        public string? TELF2 { get; set; }

        [SapName("TELFX")]
        public string? TELFX { get; set; }

       
    }
}
