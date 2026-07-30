using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{

    public class CompanyOutput
    {
        [SapName("ET_COMPANY")]
        public CompanySapModel[]? listCompanySap { get; set; }

    }

    public class CompanySapModel
    {
        [SapName("MANDT")]
        public string? mandt { get; set; }

        [SapName("SPRAS")]
        public string? spras { get; set; }

        [SapName("VKORG")]
        public string? vkorg { get; set; }

        [SapName("VTEXT")]
        public string? vtext { get; set; }
    }
}
