using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class HRExpenseWithOutTripTableDtoParameter
    {
        public int? filterUserId { get; set; }
        public string? filterName {get;set;}
        public string? filterUsername {get;set;}
        public int? filterGidisYeriId { get;set;}
        public DateTime? filterGidisTarihi { get;set;}
        public DateTime? filterDonusTarihi { get;set;}

    }
}
