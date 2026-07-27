using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class FiyatOnayi
    {

        [SapName("SELKZ")]
        public char? SELKZ { get; set; }

        [SapName("COLOR")]
        public string? COLOR { get; set; }

        [SapName("MANDT")]
        public string? MANDT { get; set; }

        [SapName("KSCHL")]
        public string? KSCHL { get; set; }

        [SapName("KNUMH")]
        public string? KNUMH { get; set; }

        [SapName("KOPOS")]
        public int? KOPOS { get; set; }

        [SapName("VKORG")]
        public string? VKORG { get; set; }

        [SapName("VTWEG")]
        public string? VTWEG { get; set; }

        [SapName("KUNWE")]
        public string? KUNWE { get; set; }

        [SapName("KUNNR")]
        public string? KUNNR { get; set; }

        [SapName("BZIRK")]
        public string? BZIRK { get; set; }

        [SapName("REGIO")]
        public string? REGIO { get; set; }

        [SapName("ZTERM")]
        public string? ZTERM { get; set; }

        [SapName("AUGRU")]
        public string? AUGRU { get; set; }

        [SapName("MATNR")]
        public string? MATNR { get; set; }

        [SapName("DATBI")]
        public string? DATBI { get; set; }

        [SapName("STEPS")]
        public int? STEPS { get; set; }

        [SapName("DATAB")]
        public string? DATAB { get; set; }

        [SapName("CHANGENR")]
        public string? CHANGENR { get; set; }

        [SapName("KBSTAT")]
        public string? KBSTAT { get; set; }

        [SapName("WI_ID")]
        public int? WI_ID { get; set; }

        [SapName("RELEASE_COMPLETE")]
        public string? RELEASE_COMPLETE { get; set; }

        [SapName("REJECTED")]
        public string? REJECTED { get; set; }

        [SapName("UNAME")]
        public string? UNAME { get; set; }

        [SapName("DATUM")]
        public string? DATUM { get; set; }

        [SapName("UZEIT")]
        public string? UZEIT { get; set; }

        [SapName("MESSAGE")]
        public string? MESSAGE { get; set; }

        [SapName("NEXT_USER")]
        public string? NEXT_USER { get; set; }

        [SapName("VALUE_NEW")]
        public string? VALUE_NEW { get; set; }

        [SapName("VALUE_OLD")]
        public string? VALUE_OLD { get; set; }

        [SapName("DBTABNAME")]
        public string? DBTABNAME { get; set; }

        [SapName("CUKY_OLD")]
        public string? CUKY_OLD { get; set; }

        [SapName("CUKY_NEW")]
        public string? CUKY_NEW { get; set; }

        [SapName("LOEKZ")]
        public string? LOEKZ { get; set; }

        [SapName("NAME1_KUNWE")]
        public string? NAME1_KUNWE { get; set; }

        [SapName("NAME1_KUNNR")]
        public string? NAME1_KUNNR { get; set; }

        [SapName("MAKTX")]
        public string? MAKTX { get; set; }

        [SapName("BZTXT")]
        public string? BZTXT { get; set; }

        [SapName("BEZEI")]
        public string? BEZEI { get; set; }
    }
}
