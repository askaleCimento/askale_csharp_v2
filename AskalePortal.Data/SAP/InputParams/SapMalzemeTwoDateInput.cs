using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class SapMalzemeTwoDateInput
    {
        [SapName("LV_TARIH1")]
        public DateTime tarih1 { get; set; }

        [SapName("LV_TARIH2")]
        public DateTime tarih2 { get; set; }
    }
}
