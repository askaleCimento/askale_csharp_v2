using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRExpenseWithOutTripTableSaveDto
    {
        public int? id{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
        public bool? enabled{get;set;}
        public int? userId{get;set;}
        public string? gidisTarihi{get;set;}
        public string? donusTarihi{get;set;}
        public int? destinationLocationId{get;set;}
        public string? digerDestination{get;set;}
        public int? tripDescriptionId{get;set;}
        public string? tripDescription{get;set;}
        public int? onaySirasi{get;set;}
        public bool? approval{get;set;}
        public bool? lastApproved{get;set;}
    }
}
