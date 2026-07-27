using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class ISGAksiyonTakipTableInputParams
    {
        [SapName("SHORT_TEXT")]
        public string? SHORT_TEXT { get; set; }

        [SapName("PRIORITY")]
        public string? PRIORITY { get; set; }

        [SapName("NOTIF_DATE")]
        public string? NOTIF_DATE { get; set; }

        [SapName("NOTIFTIME")]
        public string? NOTIFTIME { get; set; }

        [SapName("REPORTEDBY")]
        public string? REPORTEDBY { get; set; }

        [SapName("PLANPLANT")]
        public string? PLANPLANT { get; set; }

        [SapName("TEXT_LINE")]
        public string? TEXT_LINE { get; set; }
    }
}
