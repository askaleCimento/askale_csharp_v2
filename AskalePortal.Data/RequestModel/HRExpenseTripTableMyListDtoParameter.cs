using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class HRExpenseTripTableMyListDtoParameter
    {
        public int? userId { get; set; }
        public int? filterUser { get; set; }
        public int? filterDestination { get; set; }
        public int? filterWhereYouAre { get; set; }
        public DateTime? filterGidisTarihi { get; set; }
        public DateTime? filterDonusTarihi { get; set; }

    }
}
