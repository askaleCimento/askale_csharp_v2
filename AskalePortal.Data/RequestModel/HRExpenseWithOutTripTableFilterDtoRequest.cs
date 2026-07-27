using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class HRExpenseWithOutTripTableFilterDtoRequest
    {
        public string? filterName{get;set;}
        public string? filterUsername{get;set;}
        public int? filterUserId{get;set;}
    }
}
