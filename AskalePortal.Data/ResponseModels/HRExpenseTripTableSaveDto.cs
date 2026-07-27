using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRExpenseTripTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? destinationLocationId{get;set;}
        public int? hereLocationId{get;set;}
        public int? tripDescriptionId{get;set;}
        public int? userId{get;set;}
        public int? currentUserId{get;set;}
        public int? vekaletId{get;set;}
        public string? gidisTarihi{get;set;}
        public string? donusTarihi{get;set;}
        public string? digerDestination{get;set;}
        public string? tripDescription{get;set;}
        public decimal? avans{get;set;}
        public int? onaySirasi{get;set;}
        public bool? lastApproved{get;set;}
        public bool? approval{get;set;}
        public int? currentStateId{get;set;}
        public string? disaprovecondition{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? updateDate{get;set;}
    }
}
