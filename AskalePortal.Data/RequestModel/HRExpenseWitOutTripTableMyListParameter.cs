using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class HRExpenseWitOutTripTableMyListParameter
    {
        public int? userId {  get; set; }
        public DateTime? gidisTarihi { get; set; }
        public DateTime? donusTarihi { get; set; }
        public int? filterDestination { get; set; }
        public int? filterUserId { get; set; }

    }
}
