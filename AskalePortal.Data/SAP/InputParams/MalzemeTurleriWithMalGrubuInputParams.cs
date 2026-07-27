using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class MalzemeTurleriWithMalGrubuInputParams
    {
        [SapName("IV_SIRKET")]
        public string? IV_SIRKET { get; set; }

        [SapName("IV_MALGRUP")]
        public string? IV_MALGRUP { get; set; }
    }
}
