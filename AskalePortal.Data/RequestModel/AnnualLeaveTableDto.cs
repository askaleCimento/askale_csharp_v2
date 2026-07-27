using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class AnnualLeaveTableDto
    {
        public int? id { get; set; }
        public string? username { get; set; }
        public double? kalanIzin { get; set; }
        public double? istenenIzin { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public int? currentStateId { get; set; }
    }
}
