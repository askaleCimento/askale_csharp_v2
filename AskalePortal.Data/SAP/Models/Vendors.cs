using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
	public class Vendors
	{


        [SapName("LIFNR")]
        public string? LIFNR { get; set; }

        [SapName("LAND1")]
        public string? LAND1 { get; set; }

        [SapName("NAME1")]
        public string? NAME1 { get; set; }

        [SapName("NAME2")]
        public string? NAME2 { get; set; }

        [SapName("NAME3")]
        public string? NAME3 { get; set; }

        [SapName("NAME4")]
        public string? NAME4 { get; set; }

        [SapName("TELF1")]
        public string? TELF1 { get; set; }
		
	}
}
