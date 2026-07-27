using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ReportDataset
{
    public class TemsiliHarcamaDataSource
    {
        public int id { get; set; }
        public string user { get; set; }
        public string harcamaTuru { get; set; }
        public DateTime harcamaTarihi { get; set; }
        public string aciklama { get; set; }
        public decimal tutar { get; set; }
        public decimal onaylananTutar { get; set; }
        public string durum { get; set; }
    }
}
