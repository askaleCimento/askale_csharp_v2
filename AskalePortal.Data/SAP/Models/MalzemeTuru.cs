using Newtonsoft.Json;
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
        public string? matnr { get; set; }

        [SapName("MAKTX")]
        public string? maktx { get; set; }

        [SapName("MATKL")]
        public string? matkl { get; set; }

        [SapName("WERKS")]
        public string? werks { get; set; }
    }
}
