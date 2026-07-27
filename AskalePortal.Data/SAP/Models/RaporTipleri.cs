using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class RaporTipleri
    {
        [SapName("MANDT")]
       public string? MANDT    {get;set;}

        [SapName("RAPORADI")]
        public string? RAPORADI {get;set;}

        [SapName("RAPORTIPI")]
        public string? RAPORTIPI { get; set; }
    }
}
