using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ApprovalProcessDetailSaveDto
    {
        public int? id { get; set; }
        public bool? enabled { get; set; }
        public int? createdUserId { get; set; }
        public string? createdDate { get; set; }
        public string? updateDate { get; set; }
        public int? updatedUserId { get; set; }
        public int? processId { get; set; }
        public int? userId { get; set; }
        public int? vekaletId { get; set; }
        public dynamic? deger { get; set; }
        public int? dataOrder { get; set; }
    }
}
