using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class RepresentativeExpenseTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public int? userId{get;set;}
        public int? currentUserId{get;set;}
        public int? currentStateId{get;set;}
        public string? spendingTime{get;set;}
        public int? typeId{get;set;}
        public string? description{get;set;}
        public decimal? amount{get;set;}
        public decimal? approvedAmount{get;set;}
        public bool? approval{get;set;}
        public int? onaySirasi{get;set;}
        public string? fileNames{get;set;}
        public string? disaproveCondition{get;set;}
    }
}
