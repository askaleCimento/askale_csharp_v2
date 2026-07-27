using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class ChequeStatus
    {
        [SapName("OUTPUT")]
        public ChequeStatusModel[]? listChequeStatusModel { get; set; }

    }
    public class ChequeStatusModel
    {
        [SapName("BUKRS")]
        public string? bukrs{get;set;}

        [SapName("WBZOG")]
        public string? wbzog{get;set;}

        [SapName("BELNR")]
        public string? belnr{get;set;}

        [SapName("BUZEI")]
        public string? buzei{get;set;}

        [SapName("ZFBDT")]
        public string? zfbdt{get;set;}

        [SapName("WVERW")]
        public string? wverw{get;set;}

        [SapName("SGTXT2")]
        public string? sgtxt2{get;set;}

        [SapName("DURUM")]
        public string? durum{get;set;}

        [SapName("DENK")]
        public string? denk{get;set;}

        [SapName("DMBTR")]
        public string? dmbtr{get;set;}
    }
}
