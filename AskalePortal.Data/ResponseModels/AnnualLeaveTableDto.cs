using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class AnnualLeaveTableResponseDto
    {
        public int? id { get; set; }
        public string? username { get; set; }
        public decimal kalanIzin { get; set; }
        public decimal istenenIzin { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public int currentStateId { get; set; }
    }
}
