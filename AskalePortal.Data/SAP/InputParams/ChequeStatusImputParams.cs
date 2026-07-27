using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class ChequeStatusImputParams
    {

        [SapName("TARIH")]
        public required string tarihP { get; set; }

    }
}
