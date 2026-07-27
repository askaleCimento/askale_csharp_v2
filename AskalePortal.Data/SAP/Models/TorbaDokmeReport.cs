using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public partial class TorbaDokmeReport
    {
        [SapName("MANDT")]
        public string? mandt { get; set; }

        [SapName("TARIH")]
        public DateTime? tarih { get; set; }

        [SapName("WERKS")]
        public string? werks { get; set; }

        [SapName("GSAAT")]
        public string? gsaat { get; set; }

        [SapName("ZDURUM_TEXT")]
        public string? zdurumtext { get; set; }

        [SapName("TORBA")]
        public double? torba { get; set; }

        [SapName("DOKME")]
        public double? dokme { get; set; }

        [SapName("TOPLAM")]
        public double? toplam { get; set; }

        [SapName("ZTARTBRM")]
        public string? ztartbrm { get; set; }
    }
}
