using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class ChangeCrediLimitOutput
    {
        [SapName("EV_MESSAGE ")]
        public string? EV_MESSAGE { get; set; }
        [SapName("EV_RETURN ")]
        public string? EV_RETURN { get; set; }
    }
}
