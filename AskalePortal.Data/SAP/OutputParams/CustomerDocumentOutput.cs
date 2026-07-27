using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class CustomerDocumentOutput
    {
        [SapName("OUTPUT")]
        public CustomerDocumentDto[]? OUTPUT { get; set; }
    }

    public class CustomerDocumentDto
    {
        [SapName("BUKRS")]
        public string? BUKRS { get; set; }

        [SapName("KUNNR")]
        public string? KUNNR { get; set; }

        [SapName("ZZMUS_AD")]
        public string? ZZMUS_AD { get; set; }

        [SapName("BLDAT")]
        public string? BLDAT { get; set; }

        [SapName("BUDAT")]
        public string? BUDAT { get; set; }

        [SapName("ZFBDT")]
        public string? ZFBDT { get; set; }

        [SapName("FAEDT")]
        public string? FAEDT { get; set; }

        [SapName("BELNR")]
        public string? BELNR { get; set; }

        [SapName("BLART")]
        public string? BLART { get; set; }

        [SapName("BSCHL")]
        public string? BSCHL { get; set; }

        [SapName("ICO_DUE")]
        public string? ICO_DUE { get; set; }

        [SapName("UMSKS")]
        public string? UMSKS { get; set; }

        [SapName("DMSHB")]
        public string? DMSHB { get; set; }

        [SapName("HWAER")]
        public string? HWAER { get; set; }

        [SapName("SKFBT")]
        public string? SKFBT { get; set; }

        [SapName("SGTXT")]
        public string? SGTXT { get; set; }

        [SapName("UMSKZ")]
        public string? UMSKZ { get; set; }

        [SapName("ZTERM")]
        public string? ZTERM { get; set; }

        [SapName("ZBD1T")]
        public string? ZBD1T { get; set; }

        [SapName("ZBD2T")]
        public string? ZBD2T { get; set; }

        [SapName("ZINSZ")]
        public string? ZINSZ { get; set; }

        [SapName("VERZN")]
        public string? VERZN { get; set; }

        [SapName("MWSKZ")]
        public string? MWSKZ { get; set; }

        [SapName("XBLNR")]
        public string? XBLNR { get; set; }

        [SapName("HKONT")]
        public string? HKONT { get; set; }

        [SapName("U_BKTXT")]
        public string? U_BKTXT { get; set; }

        [SapName("ZZMALHIZ")]
        public string? ZZMALHIZ { get; set; }

        [SapName("XRAGL")]
        public string? XRAGL { get; set; }

        [SapName("REBZG")]
        public string? REBZG { get; set; }

        [SapName("VBELN")]
        public string? VBELN { get; set; }

        [SapName("MONAT")]
        public string? MONAT { get; set; }

        [SapName("JAMON")]
        public string? JAMON { get; set; }

        [SapName("GJAHR")]
        public string? GJAHR { get; set; }

        [SapName("DMBE2")]
        public string? DMBE2 { get; set; }

        [SapName("HWAE2")]
        public string? HWAE2 { get; set; }

        [SapName("DMBE3")]
        public string? DMBE3 { get; set; }

        [SapName("HWAE3")]
        public string? HWAE3 { get; set; }

        [SapName("TELF1")]
        public string? TELF1 { get; set; }

        [SapName("ORT01")]
        public string? ORT01 { get; set; }

        [SapName("STCD1")]
        public string? STCD1 { get; set; }

        [SapName("STCD2")]
        public string? STCD2 { get; set; }

        [SapName("U_MATNR")]
        public string? U_MATNR { get; set; }

        [SapName("KOSTL")]
        public string? KOSTL { get; set; }

        [SapName("U_CPUDT")]
        public string? U_CPUDT { get; set; }

        [SapName("U_CPUTM")]
        public string? U_CPUTM { get; set; }

        [SapName("U_USNAM")]
        public string? U_USNAM { get; set; }

        [SapName("SNLMT")]
        public string? SNLMT { get; set; }

        [SapName("KLLMT")]
        public string? KLLMT { get; set; }
    }
}
