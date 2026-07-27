using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class HRExpenseWithOutTripTableActiveListDtoRequest
    {
        public int? userId{get;set;}
        public string? filterGidisTarihi{get;set;}
        public string? filterDonusTarihi{get;set;}
        public int? filterDestination{get;set;}
        public int? filterUserId{get;set;}
    }
}
