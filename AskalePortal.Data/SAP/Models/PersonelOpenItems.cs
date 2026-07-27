using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class PersonelOpenItems
    {

        [SapName("BUKRS")]
        public string? BUKRS { get; set; }

        [SapName("KUNNR")]
        public string? KUNNR { get; set; }

        [SapName("UMSKS")]
        public string? UMSKS { get; set; }

        [SapName("UMSKZ")]
        public string? UMSKZ { get; set; }

        [SapName("AUGDT")]
        public string? AUGDT { get; set; }

        [SapName("AUGBL")]
        public string? AUGBL { get; set; }

        [SapName("ZUONR")]
        public string? ZUONR { get; set; }

        [SapName("GJAHR")]
        public string? GJAHR { get; set; }

        [SapName("BELNR")]
        public string? BELNR { get; set; }

        [SapName("BUZEI")]
        public string? BUZEI { get; set; }

        [SapName("BUDAT")]
        public DateTime? BUDAT { get; set; }

        [SapName("BLDAT")]
        public DateTime? BLDAT { get; set; }

        [SapName("CPUDT")]
        public DateTime? CPUDT { get; set; }

        [SapName("WAERS")]
        public string? WAERS { get; set; }

        [SapName("XBLNR")]
        public string? XBLNR { get; set; }

        [SapName("BLART")]
        public string? BLART { get; set; }

        [SapName("MONAT")]
        public string? MONAT { get; set; }

        [SapName("BSCHL")]
        public string? BSCHL { get; set; }

        [SapName("ZUMSK")]
        public string? ZUMSK { get; set; }

        [SapName("SHKZG")]
        public string? SHKZG { get; set; }

        [SapName("GSBER")]
        public string? GSBER { get; set; }

        [SapName("MWSKZ")]
        public string? MWSKZ { get; set; }

        [SapName("DMBTR")]
        public double? DMBTR { get; set; }

        [SapName("WRBTR")]
        public string? WRBTR { get; set; }

        [SapName("MWSTS")]
        public string? MWSTS { get; set; }

        [SapName("WMWST")]
        public string? WMWST { get; set; }

        [SapName("BDIFF")]
        public string? BDIFF { get; set; }

        [SapName("BDIF2")]
        public string? BDIF2 { get; set; }

        [SapName("PROJN")]
        public string? PROJN { get; set; }

        [SapName("AUFNR")]
        public string? AUFNR { get; set; }

        [SapName("ANLN1")]
        public string? ANLN1 { get; set; }

        [SapName("ANLN2")]
        public string? ANLN2 { get; set; }

        [SapName("SAKNR")]
        public string? SAKNR { get; set; }

        [SapName("HKONT")]
        public string? HKONT { get; set; }

        [SapName("FKONT")]
        public string? FKONT { get; set; }

        [SapName("FILKD")]
        public string? FILKD { get; set; }

        [SapName("ZFBDT")]
        public string? ZFBDT { get; set; }

        [SapName("ZTERM")]
        public string? ZTERM { get; set; }

        [SapName("ZBD1T")]
        public string? ZBD1T { get; set; }

        [SapName("ZBD2T")]
        public string? ZBD2T { get; set; }

        [SapName("ZBD3T")]
        public string? ZBD3T { get; set; }

        [SapName("ZBD1P")]
        public string? ZBD1P { get; set; }

        [SapName("ZBD2P")]
        public string? ZBD2P { get; set; }

        [SapName("SKFBT")]
        public string? SKFBT { get; set; }

        [SapName("SKNTO")]
        public string? SKNTO { get; set; }

        [SapName("WSKTO")]
        public string? WSKTO { get; set; }

        [SapName("ZLSCH")]
        public string? ZLSCH { get; set; }

        [SapName("ZLSPR")]
        public string? ZLSPR { get; set; }

        [SapName("ZBFIX")]
        public string? ZBFIX { get; set; }

        [SapName("HBKID")]
        public string? HBKID { get; set; }

        [SapName("BVTYP")]
        public string? BVTYP { get; set; }

        [SapName("REBZG")]
        public string? REBZG { get; set; }

        [SapName("REBZJ")]
        public string? REBZJ { get; set; }

        [SapName("REBZZ")]
        public string? REBZZ { get; set; }

        [SapName("SAMNR")]
        public string? SAMNR { get; set; }

        [SapName("ANFBN")]
        public string? ANFBN { get; set; }

        [SapName("ANFBJ")]
        public string? ANFBJ { get; set; }

        [SapName("ANFBU")]
        public string? ANFBU { get; set; }

        [SapName("ANFAE")]
        public string? ANFAE { get; set; }

        [SapName("MANSP")]
        public string? MANSP { get; set; }

        [SapName("MSCHL")]
        public string? MSCHL { get; set; }

        [SapName("MADAT")]
        public string? MADAT { get; set; }

        [SapName("MANST")]
        public string? MANST { get; set; }

        [SapName("MABER")]
        public string? MABER { get; set; }

        [SapName("XNETB")]
        public string? XNETB { get; set; }

        [SapName("XANET")]
        public string? XANET { get; set; }

        [SapName("XCPDD")]
        public string? XCPDD { get; set; }

        [SapName("XINVE")]
        public string? XINVE { get; set; }

        [SapName("XZAHL")]
        public string? XZAHL { get; set; }

        [SapName("MWSK1")]
        public string? MWSK1 { get; set; }

        [SapName("DMBT1")]
        public string? DMBT1 { get; set; }

        [SapName("WRBT1")]
        public string? WRBT1 { get; set; }

        [SapName("MWSK2")]
        public string? MWSK2 { get; set; }

        [SapName("DMBT2")]
        public string? DMBT2 { get; set; }

        [SapName("WRBT2")]
        public string? WRBT2 { get; set; }

        [SapName("MWSK3")]
        public string? MWSK3 { get; set; }

        [SapName("DMBT3")]
        public string? DMBT3 { get; set; }

        [SapName("WRBT3")]
        public string? WRBT3 { get; set; }

        [SapName("BSTAT")]
        public string? BSTAT { get; set; }

        [SapName("VBUND")]
        public string? VBUND { get; set; }

        [SapName("VBELN")]
        public string? VBELN { get; set; }

        [SapName("REBZT")]
        public string? REBZT { get; set; }

        [SapName("INFAE")]
        public string? INFAE { get; set; }

        [SapName("STCEG")]
        public string? STCEG { get; set; }

        [SapName("EGBLD")]
        public string? EGBLD { get; set; }

        [SapName("EGLLD")]
        public string? EGLLD { get; set; }

        [SapName("RSTGR")]
        public string? RSTGR { get; set; }

        [SapName("XNOZA")]
        public string? XNOZA { get; set; }

        [SapName("VERTT")]
        public string? VERTT { get; set; }

        [SapName("VERTN")]
        public string? VERTN { get; set; }

        [SapName("VBEWA")]
        public string? VBEWA { get; set; }

        [SapName("WVERW")]
        public string? WVERW { get; set; }

        [SapName("PROJK")]
        public string? PROJK { get; set; }

        [SapName("FIPOS")]
        public string? FIPOS { get; set; }

        [SapName("NPLNR")]
        public string? NPLNR { get; set; }

        [SapName("AUFPL")]
        public string? AUFPL { get; set; }

        [SapName("APLZL")]
        public string? APLZL { get; set; }

        [SapName("XEGDR")]
        public string? XEGDR { get; set; }

        [SapName("DMBE2")]
        public string? DMBE2 { get; set; }

        [SapName("DMBE3")]
        public string? DMBE3 { get; set; }

        [SapName("DMB21")]
        public string? DMB21 { get; set; }

        [SapName("DMB22")]
        public string? DMB22 { get; set; }

        [SapName("DMB23")]
        public string? DMB23 { get; set; }

        [SapName("DMB31")]
        public string? DMB31 { get; set; }

        [SapName("DMB32")]
        public string? DMB32 { get; set; }

        [SapName("DMB33")]
        public string? DMB33 { get; set; }

        [SapName("BDIF3")]
        public string? BDIF3 { get; set; }

        [SapName("XRAGL")]
        public string? XRAGL { get; set; }

        [SapName("UZAWE")]
        public string? UZAWE { get; set; }

        [SapName("XSTOV")]
        public string? XSTOV { get; set; }

        [SapName("MWST2")]
        public string? MWST2 { get; set; }

        [SapName("MWST3")]
        public string? MWST3 { get; set; }

        [SapName("SKNT2")]
        public string? SKNT2 { get; set; }

        [SapName("SKNT3")]
        public string? SKNT3 { get; set; }

        [SapName("XREF1")]
        public string? XREF1 { get; set; }

        [SapName("XREF2")]
        public string? XREF2 { get; set; }

        [SapName("XARCH")]
        public string? XARCH { get; set; }

        [SapName("PSWSL")]
        public string? PSWSL { get; set; }

        [SapName("PSWBT")]
        public string? PSWBT { get; set; }

        [SapName("LZBKZ")]
        public string? LZBKZ { get; set; }

        [SapName("LANDL")]
        public string? LANDL { get; set; }

        [SapName("IMKEY")]
        public string? IMKEY { get; set; }

        [SapName("VBEL2")]
        public string? VBEL2 { get; set; }

        [SapName("VPOS2")]
        public string? VPOS2 { get; set; }

        [SapName("POSN2")]
        public string? POSN2 { get; set; }

        [SapName("ETEN2")]
        public string? ETEN2 { get; set; }

        [SapName("FISTL")]
        public string? FISTL { get; set; }

        [SapName("GEBER")]
        public string? GEBER { get; set; }

        [SapName("DABRZ")]
        public string? DABRZ { get; set; }

        [SapName("XNEGP")]
        public string? XNEGP { get; set; }

        [SapName("KOSTL")]
        public string? KOSTL { get; set; }

        [SapName("RFZEI")]
        public string? RFZEI { get; set; }

        [SapName("KKBER")]
        public string? KKBER { get; set; }

        [SapName("EMPFB")]
        public string? EMPFB { get; set; }

        [SapName("PRCTR")]
        public string? PRCTR { get; set; }

        [SapName("XREF3")]
        public string? XREF3 { get; set; }

        [SapName("QSSKZ")]
        public string? QSSKZ { get; set; }

        [SapName("ZINKZ")]
        public string? ZINKZ { get; set; }

        [SapName("DTWS1")]
        public string? DTWS1 { get; set; }

        [SapName("DTWS2")]
        public string? DTWS2 { get; set; }

        [SapName("DTWS3")]
        public string? DTWS3 { get; set; }

        [SapName("DTWS4")]
        public string? DTWS4 { get; set; }

        [SapName("XPYPR")]
        public string? XPYPR { get; set; }

        [SapName("KIDNO")]
        public string? KIDNO { get; set; }

        [SapName("ABSBT")]
        public string? ABSBT { get; set; }

        [SapName("CCBTC")]
        public string? CCBTC { get; set; }

        [SapName("PYCUR")]
        public string? PYCUR { get; set; }

        [SapName("PYAMT")]
        public string? PYAMT { get; set; }

        [SapName("BUPLA")]
        public string? BUPLA { get; set; }

        [SapName("SECCO")]
        public string? SECCO { get; set; }

        [SapName("CESSION_KZ")]
        public string? CESSION_KZ { get; set; }

        [SapName("PPDIFF")]
        public string? PPDIFF { get; set; }

        [SapName("PPDIF2")]
        public string? PPDIF2 { get; set; }

        [SapName("PPDIF3")]
        public string? PPDIF3 { get; set; }

        [SapName("KBLNR")]
        public string? KBLNR { get; set; }

        [SapName("KBLPOS")]
        public string? KBLPOS { get; set; }

        [SapName("GRANT_NBR")]
        public string? GRANT_NBR { get; set; }

        [SapName("GMVKZ")]
        public string? GMVKZ { get; set; }

        [SapName("SRTYPE")]
        public string? SRTYPE { get; set; }

        [SapName("LOTKZ")]
        public string? LOTKZ { get; set; }

        [SapName("FKBER")]
        public string? FKBER { get; set; }

        [SapName("INTRENO")]
        public string? INTRENO { get; set; }

        [SapName("PPRCT")]
        public string? PPRCT { get; set; }

        [SapName("BUZID")]
        public string? BUZID { get; set; }

        [SapName("AUGGJ")]
        public string? AUGGJ { get; set; }

        [SapName("HKTID")]
        public string? HKTID { get; set; }

        [SapName("BUDGET_PD")]
        public string? BUDGET_PD { get; set; }

        [SapName("KONTT")]
        public string? KONTT { get; set; }

        [SapName("KONTL")]
        public string? KONTL { get; set; }

        [SapName("UEBGDAT")]
        public string? UEBGDAT { get; set; }

        [SapName("VNAME")]
        public string? VNAME { get; set; }

        [SapName("EGRUP")]
        public string? EGRUP { get; set; }

        [SapName("BTYPE")]
        public string? BTYPE { get; set; }

        [SapName("PROPMANO")]
        public string? PROPMANO { get; set; }
    }
}
