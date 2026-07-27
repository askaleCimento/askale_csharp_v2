using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class AnnualLeaveSap
    {
        [SapName("OUTPUT")]
        public AnnualLeaveSapModel[]? listAnualLeaveSap { get; set; }

    }

    public class AnnualLeaveSapModel
    {
        [SapName("BUKRS")]
        public string? bukrs { get; set; }
        [SapName("BUTXT")]
        public string? butxt { get; set; }
        [SapName("PERNR")]
        public string? pernr { get; set; }
        [SapName("ENAME")]
        public string? ename { get; set; }
        [SapName("WERKS")]
        public string? werks { get; set; }
        [SapName("PBTXT")]
        public string? pbtxt { get; set; }
        [SapName("BTRTL")]
        public string? btrtl { get; set; }
        [SapName("BTRTX")]
        public string? btrtx { get; set; }
        [SapName("PERSK")]
        public string? persk { get; set; }
        [SapName("PTEXT")]
        public string? ptext { get; set; }
        [SapName("ORGEH")]
        public string? orgeh { get; set; }
        [SapName("ORGTX")]
        public string? orgtx { get; set; }
        [SapName("PLANS")]
        public string? plans { get; set; }
        [SapName("PLSTX")]
        public string? plstx { get; set; }
        [SapName("KOSTL")]
        public string? kostl { get; set; }
        [SapName("KOSTX")]
        public string? kostx { get; set; }
        [SapName("GBDAT")]
        public string? gbdat { get; set; }
        [SapName("HIRED")]
        public string? hired { get; set; }
        [SapName("STAT")]
        public string? stat { get; set; }
        [SapName("QUABS")]
        public string? quabs { get; set; }
        [SapName("REABS")]
        public string? reabs { get; set; }
        [SapName("USABS")]
        public string? usabs { get; set; }
        [SapName("TCABS")]
        public string? tcabs { get; set; }
        [SapName("ECABS")]
        public string? ecabs { get; set; }
        [SapName("UCABS")]
        public string? ucabs { get; set; }
    }
}
