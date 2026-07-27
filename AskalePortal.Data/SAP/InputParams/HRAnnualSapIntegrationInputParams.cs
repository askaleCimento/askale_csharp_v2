using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class HRAnnualSapIntegrationInputParams
    {
        [SapName("PERNR")]
        public string? PERNR { get; set; }

        [SapName("IZINBASLANGICI")]
        public string? IZINBASLANGICI { get; set; }

        [SapName("IZINBITISI")]
        public string? IZINBITISI { get; set; }

        [SapName("SAAT")]
        public string? SAAT { get; set; }

        [SapName("DUZELTME")]
        public string? DUZELTME { get; set; }

        [SapName("IZINTURU")]
        public string? IZINTURU { get; set; }
    }
}
