using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class IsgAksiyonTablePageableListParameter
    {
        public int? id { get; set; }
        public int? companyId { get; set; }
        public int? uygunsuzlukKaynagiId { get; set; }
        public int? userId { get;set; }
        public int? uniteId { get;set; }
        public string? aciklama { get; set; }
        public bool? bittiMi { get; set; }
        public string? baslangicTarihi { get; set; }
        public string? bitisTarihi { get; set; }

        public string? uygunsuzlukTarihi { get; set; }

    }
}
