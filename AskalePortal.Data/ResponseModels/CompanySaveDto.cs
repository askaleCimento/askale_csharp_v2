using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class CompanySaveDto
    {
        public int? id { get; set; }
        public bool? enabled { get; set; }
        public int? createdUserId { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updateDate { get; set; }
        public int? updatedUserId { get; set; }
        public string? mandt { get; set; }
        public string? spras { get; set; }
        public string? vkorg { get; set; }
        public string? vtext { get; set; }
        public int? companySectionId { get; set; }
        public string? imgUrl { get; set; }
        public string? companyTitle { get; set; }
        public string? companyLongName { get; set; }
        public string? companyShortName { get; set; }
    }
}
