using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class IVTarih
    {
        [SapName("IV_TARIH")]
        public string? IV_TARIH { get; set; }
    }
}
