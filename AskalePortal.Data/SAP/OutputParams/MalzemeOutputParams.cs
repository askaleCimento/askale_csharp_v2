using AskalePortal.Data.SAP.Models;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class MalzemeOutputParams
    {
        [SapName("OUTPUT")]
        public MalzemeTuru[]? listMalzeme {  get; set; }
    }
}
