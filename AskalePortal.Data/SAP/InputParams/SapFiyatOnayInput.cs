using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class SapFiyatOnayInput
    {
        [SapName("IV_WI_ID")]
        public int iv_wi_id { get; set; }

        [SapName("IV_ONAY")]
        public int onay { get; set; }
    }
}
