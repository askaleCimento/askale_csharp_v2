using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class SirketlerOutput
    {
        [SapName("ET_COMPANY")]
        public IEnumerable<CompanySap>? Companies { get; set; }
    }
    public class CompanySap
    {
        public string? MANDT { get; set; }
        public string? SPRAS { get; set; }
        public string? VKORG { get; set; }
        public string? VTEXT { get; set; }
    }
}
