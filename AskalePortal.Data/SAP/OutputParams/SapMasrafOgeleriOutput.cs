using AskalePortal.Data.SAP.Models;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class SapMasrafOgeleriOutput
    {
        [SapName("LT_T01")]
        public MasrafOgeleri[]? masrafOgeleri { get; set; }
    }
}
