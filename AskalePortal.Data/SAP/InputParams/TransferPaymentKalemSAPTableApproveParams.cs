using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class TransferPaymentKalemSAPTableApproveParams
    {
        [SapName("LV_HENUM")]
        public required string henum { get; set; }

        [SapName("LV_POSNR")]
        public required string posnr { get; set; }

        [SapName("LV_ONAYLAYAN")]
        public required string onaylayan { get; set; }

        [SapName("LV_TARIH")]
        public required string tarih { get; set; }

        [SapName("LV_ONAYSEKLI")]
        public required string onaysekli { get; set; }

        [SapName("LV_SAAT")]
        public required string saat { get; set; }

        [SapName("API_KEY")]
        public required string apikey { get; set; }

        [SapName("BITTIMI")]
        public string? bittimi { get; set; }
    }
}
