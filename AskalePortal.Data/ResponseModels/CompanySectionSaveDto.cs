using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class CompanySectionSaveDto
    {
        public int? id { get; set; }
        public bool? enabled { get; set; }
        public int? createdUserId { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updateDate { get; set; }
        public int? updatedUserId { get; set; }
        public string? section { get; set; }
        public bool? isgGraphShow { get; set; }
    }
}
