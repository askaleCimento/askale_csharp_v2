using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.Models
{
    public partial class ActiveProcessChecks
    {
        public int? id { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updateDate { get; set; }
        public int? createdUserId { get; set; }
        public int? updatedUserId { get; set; }
        public bool? enabled { get; set; }
        public string? belnr { get; set; }
        public string? kunnr { get; set; }
        public string? name1{ get; set; }
        public string? netdt{ get; set; }
        public double? wrbtr{ get; set; }
        public int? activeProcessId { get; set; }
    }
}
