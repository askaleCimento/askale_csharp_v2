using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class SelectOption
    {
        public required string SIGN { get; set; }
        public required string OPTION { get; set; }
        public required string LOW { get; set; }
        public string? HIGH { get; set; }
    }
}
