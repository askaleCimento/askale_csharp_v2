using SapNwRfc;
using System;

namespace AskalePortal.Data.SAP.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [SapName("NAME1")]
        public string? title { get; set; }

        [SapName("KUNNR")]
        public string? KUNNR { get; set; }

        [SapName("LAND1")]
        public string? LAND1 { get; set; }

        [SapName("NAME1")]
        public string? NAME1 { get; set; }

        [SapName("NAME2")]
        public string? NAME2 { get; set; }

        [SapName("STRAS")]
        public string? STRAS { get; set; }

        [SapName("ORT01")]
        public string? ORT01 { get; set; }

        [SapName("PSTLZ")]
        public string? PSTLZ { get; set; }

        [SapName("REGIO")]
        public string? REGIO { get; set; }

        [SapName("STCD1")]
        public string? STCD1 { get; set; }
        
        [SapName("STCD2")]
        public string? STCD2 { get; set; }

        [SapName("TELF1")]
        public string? TELF1 { get; set; }

        [SapName("TELF2")]
        public string? TELF2 { get; set; }

        [SapName("TELFX")]
        public string? TELFX { get; set; }

        [SapName("ADRNR")]
        public string? ADRNR { get; set; }

        [SapName("ERDAT")]
        public DateTime? ERDAT { get; set; }

        [SapName("ERNAM")]
        public string? ERNAM { get; set; }

        [SapName("KTOKD")]
        public string? KTOKD { get; set; }

        [SapName("LIFNR")]
        public string? LIFNR { get; set; }

        [SapName("SPERR")]
        public string? SPERR { get; set; }

        [SapName("SPRAS")]
        public string? SPRAS { get; set; }

        [SapName("SORTL")]
        public string? SORTL { get; set; }

        [SapName("DUEFL")]
        public string? DUEFL { get; set; }

        [SapName("creditLimit")]
        public decimal? creditLimit { get; set; }

       
        public bool enabled { get; set; }
    }
}