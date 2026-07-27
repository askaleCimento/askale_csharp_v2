using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class SapSanalLimitIncrease
    {
       
        [SapName("LV_KUNNR")]
        public string? kunnr {  get; set; }

        [SapName("LV_DMBTR")]
        public string? dmbtr { get; set; }

        [SapName("LV_KULLANICIADI")]
        public string? kullaniciAdi { get; set; }

        [SapName("LV_YENI_MUSTERI")]
        public string? yeniMusteri { get; set; }
    }
}
