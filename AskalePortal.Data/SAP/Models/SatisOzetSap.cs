using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class SatisOzetSap
    {
        [SapName("MANDT")]
        public string? MANDT { get; set; }

        [SapName("TARIH")]
        public string? TARIH { get; set; }

        [SapName("SATORG")]
        public string? SATORG { get; set; }

        [SapName("RAPORTIPI")]
        public string? RAPORTIPI { get; set; }

        [SapName("OYIL_GUN")]
        public decimal? OYIL_GUN { get; set; }

        [SapName("CYIL_GUN")]
        public decimal? CYIL_GUN { get; set; }

        [SapName("OAY_GUN")]
        public decimal? OAY_GUN { get; set; }

        [SapName("CAY_GUN")]
        public decimal? CAY_GUN { get; set; }

        [SapName("OYIL")]
        public decimal? OYIL { get; set; }

        [SapName("CYIL")]
        public decimal? CYIL { get; set; }
    }
}
