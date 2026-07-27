using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class HRExpenseTableApprovalStatusDtoParameter
    {
        public int? userId { get; set; }
        public int? filterGidisYeriId { get; set; }
        public int? filterDonusYeriId { get; set; }
        public DateTime? filterGidisTarihi { get; set; }
        public DateTime? filterDonusTarihi { get; set; }
    }
}
