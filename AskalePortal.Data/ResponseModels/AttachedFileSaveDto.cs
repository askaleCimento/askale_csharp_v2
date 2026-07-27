using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class AttachedFileSaveDto
    {
        public int? id { get; set; }
        public bool? enabled { get; set; }
        public int? createdUserId { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updateDate { get; set; }
        public int? updatedUserId { get; set; }
        public int? moduleId { get; set; }
        public int? targetId { get; set; }
        public string? title { get; set; }
        public string? filePath { get; set; }
        public int? visitorCount { get; set; }
    }
}
