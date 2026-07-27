using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class MalzemeTuru
    {
        //[SapName("ID")]
        //public int? ID { get; set; }

        [SapName("MATNR")]
        public string? MATNR { get; set; }

        [SapName("MAKTX")]
        public string? MAKTX { get; set; }

        [SapName("MATKL")]
        public string? MATKL { get; set; }

        [SapName("WERKS")]
        public string? WERKS { get; set; }
    }
}
