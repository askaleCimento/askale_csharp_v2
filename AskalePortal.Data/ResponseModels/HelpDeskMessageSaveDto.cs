using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HelpDeskMessageSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public DateTime? createdDate{get;set;}
        public DateTime? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? message{get;set;}
        public string? createdByUserName{get;set;}
        public int? helpDeskDemandId{get;set;}
    }
}
