using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class HRExpenseInputIntoSap{

        [SapName("IM_TABLE")]
        public HRExpenseInsertedIntoSap[]? list { get; set; }
    }
    public class HRExpenseInsertedIntoSap
    {
        [SapName("PERNR")]
        public   string? PERNR { get; set; }

        [SapName("SUBTY")]
        public  string? SUBTY { get; set; }

        [SapName("ENDDA")]
        public  string? ENDDA { get; set; }
        
        [SapName("DCIYER01")]
        public string? DCIYER01 { get; set; }

        [SapName("BEGDA")]
        public  DateTime? BEGDA { get; set; }

        [SapName("AEDTM")]
        public  DateTime? AEDTM { get; set; }
       
        [SapName("UNAME")]
        public  string? UNAME { get; set; }

        [SapName("PAPER")]
        public  long? PAPER { get; set; }

        [SapName("NEDEN")]
        public  string? NEDEN { get; set; }

        [SapName("GVAYER01")]
        public  string? GVAYER01 { get; set; }

        [SapName("GVARDA01")]
        public  DateTime? GVARDA01 { get; set; }

        [SapName("GVARUZ01")]
        public  string? GVARUZ01 { get; set; }

        [SapName("DVARDA01")]
        public  DateTime? DVARDA01 { get; set; }

        [SapName("DVARUZ01")]
        public  string? DVARUZ01 { get; set; }

        [SapName("KOBES01")]
        public  string? KOBES01 { get; set; }

        [SapName("BETRG01")]
        public  decimal? BETRG01 { get; set; }

        [SapName("KOBES02")]
        public  string? KOBES02 { get; set; }

        [SapName("BETRG02")]
        public decimal? BETRG02 { get; set; }

        [SapName("KOBES03")]
        public  string? KOBES03 { get; set; }

        [SapName("BETRG03")]
        public decimal? BETRG03 { get; set; }

        [SapName("KOBES04")]
        public  string? KOBES04 { get; set; }
        
        [SapName("BETRG04")]
        public decimal? BETRG04 { get; set; }

        [SapName("KOBES05")]
        public  string? KOBES05 { get; set; }

        [SapName("BETRG05")]
        public decimal? BETRG05 { get; set; }

        [SapName("KOBES06")]
        public  string? KOBES06 { get; set; }

        [SapName("BETRG06")]
        public decimal? BETRG06 { get; set; }

        [SapName("TMAST")]
        public decimal? TMAST { get; set; }

        [SapName("WAERS")]
        public  string? WAERS { get; set; }
    }
}
