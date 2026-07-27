using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class AnnualLeave
    {
        [SapName("BUKRS")]
        public string? BUKRS {get;set;}

        [SapName("BUTXT")]
        public string? BUTXT {get;set;}

        [SapName("PERNR")]
        public string? PERNR {get;set;}

        [SapName("ENAME")]
        public string? ENAME {get;set;}

        [SapName("WERKS")]
        public string? WERKS {get;set;}

        [SapName("PBTXT")]
        public string? PBTXT {get;set;}

        [SapName("BTRTL")]
        public string? BTRTL {get;set;}

        [SapName("BTRTX")]
        public string? BTRTX {get;set;}

        [SapName("PERSK")]
        public string? PERSK {get;set;}

        [SapName("PTEXT")]
        public string? PTEXT {get;set;}

        [SapName("ORGEH")]
        public string? ORGEH {get;set;}

        [SapName("ORGTX")]
        public string? ORGTX {get;set;}

        [SapName("PLANS")]
        public string? PLANS {get;set;}

        [SapName("PLSTX")]
        public string? PLSTX {get;set;}

        [SapName("KOSTL")]
        public string? KOSTL {get;set;}

        [SapName("KOSTX")]
        public string? KOSTX { get; set; }

        [SapName("GBDAT")]
        public string? GBDAT {get;set;}

        [SapName("HIRED")]
        public string? HIRED { get; set; }

        [SapName("STAT")]
        public string? STAT  { get; set; }

        [SapName("QUABS")]
        public decimal QUABS {get;set;}

        [SapName("REABS")]
        public decimal REABS {get;set;}

        [SapName("USABS")]
        public decimal USABS {get;set;}

        [SapName("TCABS")]
        public decimal TCABS {get;set;}

        [SapName("ECABS")]
        public decimal ECABS { get; set; }

        [SapName("UCABS")]
        public decimal UCABS { get; set; }
    }
}
