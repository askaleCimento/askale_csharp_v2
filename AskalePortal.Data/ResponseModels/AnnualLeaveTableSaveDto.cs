using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class AnnualLeaveTableSaveDto
    {
        public int? id { get; set; }
        public bool? enabled { get; set; }
        public int? createdUserId { get; set; }
        public string? createdDate { get; set; }
        public string? updateDate { get; set; }
        public int? updatedUserId { get; set; }
        public int? currentUserId { get; set; }
        public int? currentStateId { get; set; }
        public int? userId { get; set; }
        public string? enteredDate { get; set; }
        public string? departmanName { get; set; }
        public string? job { get; set; }
        public int? typeId { get; set; }
        public double? dayleft { get; set; }
        public double? dayRequested { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public string? adress { get; set; }
        public int? vekaletId { get; set; }
        public int? siraNo { get; set; }
        public string? digerAciklama { get; set; }
        public string? disaprovecondition {get;set;}
    }
}
