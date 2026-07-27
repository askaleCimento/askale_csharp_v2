using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class FaturaGunFarkDto
    {
        public string? BUKRS{ get; set; }// fabrika
        public string? BELNR { get; set; } // faturano
        public int? GJAHR { get; set; } // yil
        public int? gunFarki { get; set; }
    }
}
