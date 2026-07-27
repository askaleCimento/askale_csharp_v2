using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class SUNAME
    {
        [SapName("S_UNAME")]
        public string? S_UNAME { get; set; }
    }
}
