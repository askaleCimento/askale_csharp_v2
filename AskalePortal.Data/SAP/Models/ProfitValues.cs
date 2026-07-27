using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{

    public class IlDegerler
    {

        [SapName("IL")]
        public List<string>? IL { get; set; }

        [SapName("deger")]
        public List<double>? deger { get; set; }

        [SapName("listMapProfitValues")]
        public List<MapProfitValues>? listMapProfitValues { get; set; }
    }
    public class IlceDegerler
    {

        [SapName("ILCE")]
        public string? ILCE { get; set; }

        [SapName("deger")]
        public double? deger { get; set; }
    }
    public class MapProfitValues
    {



        [SapName("KNDNR")]
        public string? KNDNR { get; set; }

        [SapName("IL")]
        public string? IL { get; set; }

        [SapName("ILCE")]
        public string? ILCE { get; set; }

        [SapName("satilanMiktarItem")]
        public double? satilanMiktarItem { get; set; }

        [SapName("urunsatilanMalinMaliyeti")]
        public double? urunsatilanMalinMaliyeti { get; set; }

        [SapName("uruntoplamIndirim")]
        public double? uruntoplamIndirim { get; set; }

        [SapName("urungelir")]
        public double? urungelir { get; set; }

        [SapName("urunprim")]
        public double? urunprim { get; set; }

        [SapName("urunAmortisman")]
        public double? urunAmortisman { get; set; }

        [SapName("urunnetKar")]
        public double? urunnetKar { get; set; }

        [SapName("urunsatisGideri")]
        public double? urunsatisGideri { get; set; }

        [SapName("urunyonetimGideri")]
        public double? urunyonetimGideri { get; set; }

        [SapName("urunticariKar")]
        public double? urunticariKar { get; set; }

        [SapName("ticariKar")]
        public double? ticariKar { get; set; }

    }
    public class ProfitValues
    {

        [SapName("BUKRS")]
        public string? BUKRS { get; set; }

        [SapName("WERKS")]
        public string? WERKS { get; set; }

        [SapName("VKORG")]
        public string? VKORG { get; set; }

        [SapName("VTWEG")]
        public string? VTWEG { get; set; }

        [SapName("KNDNR")]
        public string? KNDNR { get; set; }

        [SapName("NAME1")]
        public string? NAME1 { get; set; }

        [SapName("ARTNR")]
        public string? ARTNR { get; set; }

        [SapName("MAKTX")]
        public string? MAKTX { get; set; }

        [SapName("AKTBO")]
        public string? AKTBO { get; set; }

        [SapName("PAOBJNR")]
        public string? PAOBJNR { get; set; }

        [SapName("PASUBNR")]
        public string? PASUBNR { get; set; }

        [SapName("BISDAT")]
        public string? BISDAT { get; set; }

        [SapName("KNT_FRM_KZ")]
        public string? KNT_FRM_KZ { get; set; }

        [SapName("UNUSED_CE3")]
        public string? UNUSED_CE3 { get; set; }

        [SapName("FKART")]
        public string? FKART { get; set; }

        [SapName("KAUFN")]
        public string? KAUFN { get; set; }

        [SapName("KDPOS")]
        public string? KDPOS { get; set; }

        [SapName("AUFNR")]
        public string? AUFNR { get; set; }

        [SapName("KOKRS")]
        public string? KOKRS { get; set; }

        [SapName("GSBER")]
        public string? GSBER { get; set; }

        [SapName("SPART")]
        public string? SPART { get; set; }

        [SapName("PRCTR")]
        public string? PRCTR { get; set; }

        [SapName("PPRCTR")]
        public string? PPRCTR { get; set; }

        [SapName("KSTRG")]
        public string? KSTRG { get; set; }

        [SapName("PSPNR")]
        public string? PSPNR { get; set; }

        [SapName("KDGRP")]
        public string? KDGRP { get; set; }

        [SapName("BZIRK")]
        public string? BZIRK { get; set; }

        [SapName("IL")]
        public string? IL { get; set; }

        [SapName("ILCE")]
        public string? ILCE { get; set; }

        [SapName("VKGRP")]
        public string? VKGRP { get; set; }

        [SapName("BRSCH")]
        public string? BRSCH { get; set; }

        [SapName("LAND1")]
        public string? LAND1 { get; set; }

        [SapName("MATKL")]
        public string? MATKL { get; set; }

        [SapName("VKBUR")]
        public string? VKBUR { get; set; }

        [SapName("GEBIE")]
        public string? GEBIE { get; set; }

        [SapName("PRODH")]
        public string? PRODH { get; set; }

        [SapName("PARTNER")]
        public string? PARTNER { get; set; }

        [SapName("MAABC")]
        public string? MAABC { get; set; }

        [SapName("WW01")]
        public string? WW01 { get; set; }

        [SapName("WW02")]
        public string? WW02 { get; set; }

        [SapName("WW03")]
        public string? WW03 { get; set; }

        [SapName("KUNWE")]
        public string? KUNWE { get; set; }

        [SapName("WWBZI")]
        public string? WWBZI { get; set; }

        [SapName("ABSMG_ME")]
        public string? ABSMG_ME { get; set; }

        [SapName("VV050_ME")]
        public string? VV050_ME { get; set; }

        [SapName("VV051_ME")]
        public string? VV051_ME { get; set; }

        [SapName("REC_WAERS")]
        public string? REC_WAERS { get; set; }

        [SapName("UPDAT")]
        public string? UPDAT { get; set; }

        [SapName("USNAM")]
        public string? USNAM { get; set; }

        [SapName("PALEDGER")]
        public string? PALEDGER { get; set; }

        [SapName("VRGAR")]
        public string? VRGAR { get; set; }

        [SapName("PLIKZ")]
        public string? PLIKZ { get; set; }

        [SapName("VERSI")]
        public string? VERSI { get; set; }

        [SapName("PERBL")]
        public string? PERBL { get; set; }

        [SapName("PAPAOBJNR")]
        public string? PAPAOBJNR { get; set; }

        [SapName("PAPASUBNR")]
        public string? PAPASUBNR { get; set; }

        [SapName("HRKFT")]
        public string? HRKFT { get; set; }

        [SapName("ABSMG001")]
        public double? ABSMG001 { get; set; }

        [SapName("ERLOS001")]
        public double? ERLOS001 { get; set; }

        [SapName("VRPRS001")]
        public double? VRPRS001 { get; set; }

        [SapName("MRABA001")]
        public double? MRABA001 { get; set; }

        [SapName("PRABA001")]
        public double? PRABA001 { get; set; }

        [SapName("RABAT001")]
        public double? RABAT001 { get; set; }

        [SapName("AUSFR001")]
        public double? AUSFR001 { get; set; }

        [SapName("VSVP001")]
        public double? VSVP001 { get; set; }

        [SapName("UMSLZ001")]
        public double? UMSLZ001 { get; set; }

        [SapName("PROVV001")]
        public double? PROVV001 { get; set; }

        [SapName("VTRGK001")]
        public double? VTRGK001 { get; set; }

        [SapName("VWGK001")]
        public double? VWGK001 { get; set; }

        [SapName("VV001001")]
        public double? VV001001 { get; set; }

        [SapName("VV002001")]
        public double? VV002001 { get; set; }

        [SapName("VV003001")]
        public double? VV003001 { get; set; }

        [SapName("VV004001")]
        public double? VV004001 { get; set; }

        [SapName("VV005001")]
        public double? VV005001 { get; set; }

        [SapName("VV006001")]
        public double? VV006001 { get; set; }

        [SapName("VVSMM001")]
        public double? VVSMM001 { get; set; }

        [SapName("VV007001")]
        public double? VV007001 { get; set; }

        [SapName("VV008001")]
        public double? VV008001 { get; set; }

        [SapName("VV009001")]
        public double? VV009001 { get; set; }

        [SapName("VV010001")]
        public double? VV010001 { get; set; }

        [SapName("VV011001")]
        public double? VV011001 { get; set; }

        [SapName("VV012001")]
        public double? VV012001 { get; set; }

        [SapName("VV013001")]
        public double? VV013001 { get; set; }

        [SapName("VV014001")]
        public double? VV014001 { get; set; }

        [SapName("VV015001")]
        public double? VV015001 { get; set; }

        [SapName("VV016001")]
        public double? VV016001 { get; set; }

        [SapName("VV017001")]
        public double? VV017001 { get; set; }

        [SapName("VV018001")]
        public double? VV018001 { get; set; }

        [SapName("VV019001")]
        public double? VV019001 { get; set; }

        [SapName("VV020001")]
        public double? VV020001 { get; set; }

        [SapName("VV021001")]
        public double? VV021001 { get; set; }

        [SapName("VV022001")]
        public double? VV022001 { get; set; }

        [SapName("VV023001")]
        public double? VV023001 { get; set; }

        [SapName("VV024001")]
        public double? VV024001 { get; set; }

        [SapName("VV030001")]
        public double? VV030001 { get; set; }

        [SapName("VV031001")]
        public double? VV031001 { get; set; }

        [SapName("VV032001")]
        public double? VV032001 { get; set; }

        [SapName("VV033001")]
        public double? VV033001 { get; set; }

        [SapName("VV034001")]
        public double? VV034001 { get; set; }

        [SapName("VV040001")]
        public double? VV040001 { get; set; }

        [SapName("VV041001")]
        public double? VV041001 { get; set; }

        [SapName("VV042001")]
        public double? VV042001 { get; set; }

        [SapName("VV043001")]
        public double? VV043001 { get; set; }

        [SapName("VV044001")]
        public double? VV044001 { get; set; }

        [SapName("VV050001")]
        public double? VV050001 { get; set; }

        [SapName("VV051001")]
        public double? VV051001 { get; set; }

        [SapName("VV025001")]
        public double? VV025001 { get; set; }

        [SapName("VV026001")]
        public double? VV026001 { get; set; }

        [SapName("VV035001")]
        public double? VV035001 { get; set; }

        [SapName("VV036001")]
        public double? VV036001 { get; set; }

        [SapName("VV070001")]
        public double? VV070001 { get; set; }

        [SapName("VV071001")]
        public double? VV071001 { get; set; }

        [SapName("VV072001")]
        public double? VV072001 { get; set; }

        [SapName("VV073001")]
        public double? VV073001 { get; set; }

        [SapName("VV074001")]
        public double? VV074001 { get; set; }

        [SapName("VV075001")]
        public double? VV075001 { get; set; }

        [SapName("VV076001")]
        public double? VV076001 { get; set; }

        [SapName("VV077001")]
        public double? VV077001 { get; set; }

        [SapName("VV078001")]
        public double? VV078001 { get; set; }

        [SapName("VV079001")]
        public double? VV079001 { get; set; }

        [SapName("kontrol")]
        public bool? kontrol { get; set; }

    }

    public class YakitValues
    {

        [SapName("BUKRS")]
        public string? BUKRS { get; set; }

        [SapName("MATKL")]
        public string? MATKL { get; set; }

        [SapName("MATNR")]
        public string? MATNR { get; set; }

        [SapName("KALNR")]
        public string? KALNR { get; set; }

        [SapName("BDATJ")]
        public string? BDATJ { get; set; }

        [SapName("POPER")]
        public string? POPER { get; set; }

        [SapName("UNTPER")]
        public string? UNTPER { get; set; }

        [SapName("CATEG")]
        public string? CATEG { get; set; }

        [SapName("PTYP")]
        public string? PTYP { get; set; }

        [SapName("BVALT")]
        public string? BVALT { get; set; }

        [SapName("CURTP")]
        public string? CURTP { get; set; }

        [SapName("LBKUM")]
        public double? LBKUM { get; set; }

        [SapName("MEINS")]
        public string? MEINS { get; set; }

        [SapName("SALK3")]
        public double? SALK3 { get; set; }

        [SapName("ESTPRD")]
        public double? ESTPRD { get; set; }

        [SapName("ESTKDM")]
        public double? ESTKDM { get; set; }

        [SapName("MSTPRD")]
        public double? MSTPRD { get; set; }

        [SapName("MSTKDM")]
        public double? MSTKDM { get; set; }

        [SapName("WAERS")]
        public string? WAERS { get; set; }

        [SapName("TPPRD")]
        public double? TPPRD { get; set; }

        [SapName("ESTKDM_ST")]
        public double? ESTKDM_ST { get; set; }

    }
    public class CemClinkerRate
    {

        [SapName("POPER")]
        public string? POPER { get; set; }

        [SapName("MATNR")]
        public string? MATNR { get; set; }

        [SapName("WERKS")]
        public string? WERKS { get; set; }

        [SapName("MENGE")]
        public double? MENGE { get; set; }

        [SapName("MEINS")]
        public string? MEINS { get; set; }

        [SapName("MATKL")]
        public string? MATKL { get; set; }
    }
    public class MasrafOgeleri
    {

        [SapName("CURTP")]
        public string? CURTP { get; set; }

        [SapName("POPER")]
        public string? POPER { get; set; }

        [SapName("BDATJ")]
        public string? BDATJ { get; set; }

        [SapName("MATNR")]
        public string? MATNR { get; set; }

        [SapName("MATKL")]
        public string? MATKL { get; set; }

        [SapName("TEXT_CKMLHD_MATNR")]
        public string? TEXT_CKMLHD_MATNR { get; set; }

        [SapName("BWKEY")]
        public string? BWKEY { get; set; }

        [SapName("VERID")]
        public string? VERID { get; set; }

        [SapName("PRTYP")]
        public string? PRTYP { get; set; }

        [SapName("MENGE")]
        public double? MENGE { get; set; }

        [SapName("Z_0046")]
        public double? Z_0046 { get; set; }

        [SapName("Z_0048")]
        public double? Z_0048 { get; set; }

        [SapName("Z_0049")]
        public double? Z_0049 { get; set; }

        [SapName("Z_0050")]
        public double? Z_0050 { get; set; }

        [SapName("Z_0051")]
        public double? Z_0051 { get; set; }

        [SapName("Z_0052")]
        public double? Z_0052 { get; set; }

        [SapName("Z_0053")]
        public double? Z_0053 { get; set; }

        [SapName("Z_0054")]
        public double? Z_0054 { get; set; }

        [SapName("Z_0055")]
        public double? Z_0055 { get; set; }

        [SapName("Z_0056")]
        public double? Z_0056 { get; set; }

        [SapName("Z_0057")]
        public double? Z_0057 { get; set; }

        [SapName("Z_0058")]
        public double? Z_0058 { get; set; }

        [SapName("Z_0059")]
        public double? Z_0059 { get; set; }

        [SapName("Z_0060")]
        public double? Z_0060 { get; set; }

        [SapName("Z_0061")]
        public double? Z_0061 { get; set; }

        [SapName("Z_0062")]
        public double? Z_0062 { get; set; }

        [SapName("Z_0063")]
        public double? Z_0063 { get; set; }

        [SapName("Z_0064")]
        public double? Z_0064 { get; set; }

        [SapName("Z_0067")]
        public double? Z_0067 { get; set; }

        [SapName("Z_0068")]
        public double? Z_0068 { get; set; }

        [SapName("Z_0069")]
        public double? Z_0069 { get; set; }

        [SapName("TEXT_MKAL_VERID")]
        public string? TEXT_MKAL_VERID { get; set; }
    }
    public class ProfitReturnValues
    {


        [SapName("listProfitValues")]
        public ProfitValues[]? listProfitValues { get; set; }

        [SapName("listYakitValues")]
        public YakitValues[]? listYakitValues { get; set; }

        [SapName("listProfitDefter")]
        public ProfitDefter[]? listProfitDefter { get; set; }

        [SapName("listYakitPreviousValues")]
        public Degerler[]? listYakitPreviousValues { get; set; }

        [SapName("fabrika")]
        public string? fabrika { get; set; }

        [SapName("listMasrafOgeleri")]
        public MasrafOgeleri[]? listMasrafOgeleri { get; set; }



    }

    public class ProfitValueModel
    {



        [SapName("fabrika")]
        public string? fabrika { get; set; }

        [SapName("genelKarlilik")]
        public List<string>? genelKarlilik { get; set; }

        [SapName("genelKarlilikLabel")]
        public List<string>? genelKarlilikLabel { get; set; }

        [SapName("fabrikagenelKarlilik")]
        public List<string>? fabrikagenelKarlilik { get; set; }

        [SapName("fabrikagenelKarlilikLabel")]
        public List<string>? fabrikagenelKarlilikLabel { get; set; }

        [SapName("urunKarlilik")]
        public List<string>? urunKarlilik { get; set; }

        [SapName("urunKarlilikLabel")]
        public List<string>? urunKarlilikLabel { get; set; }

        [SapName("musteriKarlilik")]
        public List<string>? musteriKarlilik { get; set; }

        [SapName("musteriKarlilikLabel")]
        public List<string>? musteriKarlilikLabel { get; set; }

        [SapName("musteriKarlilikNet")]
        public List<string>? musteriKarlilikNet { get; set; }

        [SapName("musteriKarlilikTonaj")]
        public List<string>? musteriKarlilikTonaj { get; set; }

        [SapName("urunMaliyet")]
        public List<string>? urunMaliyet { get; set; }

        [SapName("urunMaliyetLabel")]
        public List<string>? urunMaliyetLabel { get; set; }

        [SapName("urunTicariMaliyet")]
        public List<string>? urunTicariMaliyet { get; set; }

        [SapName("urunTicariMaliyetLabel")]
        public List<string>? urunTicariMaliyetLabel { get; set; }

        [SapName("yakitMaliyetOnceki")]
        public List<string>? yakitMaliyetOnceki { get; set; }

        [SapName("yakitMaliyetOncekiLabel")]
        public List<string>? yakitMaliyetOncekiLabel { get; set; }

        [SapName("yakitMaliyet")]
        public List<string>? yakitMaliyet { get; set; }

        [SapName("yakitMaliyetLabel")]
        public List<string>? yakitMaliyetLabel { get; set; }

        [SapName("yakitMaliyetDagilim")]
        public List<string>? yakitMaliyetDagilim { get; set; }

        [SapName("yakitMaliyetDagilimLabel")]
        public List<string>? yakitMaliyetDagilimLabel { get; set; }

        [SapName("yakitMaliyetDagilimOnceki")]
        public List<string>? yakitMaliyetDagilimOnceki { get; set; }

        [SapName("yakitMaliyetDagilimOncekiLabel")]
        public List<string>? yakitMaliyetDagilimOncekiLabel { get; set; }

        [SapName("yakitMaliyetDagilimRatio")]
        public List<string>? yakitMaliyetDagilimRatio { get; set; }

        [SapName("yakitMaliyetDagilimRatioLabel")]
        public List<string>? yakitMaliyetDagilimRatioLabel { get; set; }

        [SapName("yakitMaliyetDagilimOncekiRatio")]
        public List<string>? yakitMaliyetDagilimOncekiRatio { get; set; }

        [SapName("yakitMaliyetDagilimOncekiRatioLabel")]
        public List<string>? yakitMaliyetDagilimOncekiRatioLabel { get; set; }

        [SapName("yakitValues1")]
        public List<YakitValues>? yakitValues1 { get; set; }

        [SapName("yakitValues2")]
        public List<YakitValues>? yakitValues2 { get; set; }

        [SapName("yakitValues3")]
        public List<YakitValues>? yakitValues3 { get; set; }

        [SapName("yakitValues4")]
        public List<YakitValues>? yakitValues4 { get; set; }

        [SapName("tarih")]
        public DateTime? tarih { get; set; }

        [SapName("Durum")]
        public bool? Durum { get; set; }

        [SapName("cemClinkerRateOnceki")]
        public List<string>? cemClinkerRateOnceki { get; set; }

        [SapName("cemClinkerRateOncekiLabel")]
        public List<string>? cemClinkerRateOncekiLabel { get; set; }

        [SapName("cemClinkerRate")]
        public List<string>? cemClinkerRate { get; set; }

        [SapName("cemClinkerRateLabel")]
        public List<string>? cemClinkerRateLabel { get; set; }


    }


    public class ProfitDefter
    {

        [SapName("RYEAR")]
        public string? RYEAR { get; set; }

        [SapName("OBJNR00")]
        public string? OBJNR00 { get; set; }

        [SapName("OBJNR01")]
        public string? OBJNR01 { get; set; }

        [SapName("OBJNR02")]
        public string? OBJNR02 { get; set; }

        [SapName("OBJNR03")]
        public string? OBJNR03 { get; set; }

        [SapName("OBJNR04")]
        public string? OBJNR04 { get; set; }

        [SapName("OBJNR05")]
        public string? OBJNR05 { get; set; }

        [SapName("OBJNR06")]
        public string? OBJNR06 { get; set; }

        [SapName("OBJNR07")]
        public string? OBJNR07 { get; set; }

        [SapName("OBJNR08")]
        public string? OBJNR08 { get; set; }

        [SapName("DRCRK")]
        public string? DRCRK { get; set; }

        [SapName("RPMAX")]
        public string? RPMAX { get; set; }

        [SapName("ACTIV")]
        public string? ACTIV { get; set; }

        [SapName("RMVCT")]
        public string? RMVCT { get; set; }

        [SapName("RTCUR")]
        public string? RTCUR { get; set; }

        [SapName("RUNIT")]
        public string? RUNIT { get; set; }

        [SapName("AWTYP")]
        public string? AWTYP { get; set; }

        [SapName("RLDNR")]
        public string? RLDNR { get; set; }

        [SapName("RRCTY")]
        public string? RRCTY { get; set; }

        [SapName("RVERS")]
        public string? RVERS { get; set; }

        [SapName("LOGSYS")]
        public string? LOGSYS { get; set; }

        [SapName("RACCT")]
        public string? RACCT { get; set; }

        [SapName("COST_ELEM")]
        public string? COST_ELEM { get; set; }

        [SapName("RBUKRS")]
        public string? RBUKRS { get; set; }

        [SapName("RCNTR")]
        public string? RCNTR { get; set; }

        [SapName("PRCTR")]
        public string? PRCTR { get; set; }

        [SapName("RFAREA")]
        public string? RFAREA { get; set; }

        [SapName("RBUSA")]
        public string? RBUSA { get; set; }

        [SapName("KOKRS")]
        public string? KOKRS { get; set; }

        [SapName("SEGMENT")]
        public string? SEGMENT { get; set; }

        [SapName("SCNTR")]
        public string? SCNTR { get; set; }

        [SapName("PPRCTR")]
        public string? PPRCTR { get; set; }

        [SapName("SFAREA")]
        public string? SFAREA { get; set; }

        [SapName("SBUSA")]
        public string? SBUSA { get; set; }

        [SapName("RASSC")]
        public string? RASSC { get; set; }

        [SapName("PSEGMENT")]
        public string? PSEGMENT { get; set; }

        [SapName("TSLVT")]
        public double? TSLVT { get; set; }

        [SapName("HSL01")]
        public double? HSL01 { get; set; }

        [SapName("HSL02")]
        public double? HSL02 { get; set; }

        [SapName("HSL03")]
        public double? HSL03 { get; set; }

        [SapName("HSL04")]
        public double? HSL04 { get; set; }

        [SapName("HSL05")]
        public double? HSL05 { get; set; }

        [SapName("HSL06")]
        public double? HSL06 { get; set; }

        [SapName("HSL07")]
        public double? HSL07 { get; set; }

        [SapName("HSL08")]
        public double? HSL08 { get; set; }

        [SapName("HSL09")]
        public double? HSL09 { get; set; }

        [SapName("HSL10")]
        public double? HSL10 { get; set; }

        [SapName("HSL11")]
        public double? HSL11 { get; set; }

        [SapName("HSL12")]
        public double? HSL12 { get; set; }

        [SapName("HSL13")]
        public double? HSL13 { get; set; }

        [SapName("HSL14")]
        public double? HSL14 { get; set; }

        [SapName("HSL15")]
        public double? HSL15 { get; set; }

        [SapName("HSL16")]
        public double? HSL16 { get; set; }

    }
    public class Degerler
    {

        [SapName("name")]
        public string? name { get; set; }

        [SapName("deger")]
        public double? deger { get; set; }

        [SapName("karlilikNet")]
        public double? karlilikNet { get; set; }

        [SapName("tonaj")]
        public double? tonaj { get; set; }
    }

}
