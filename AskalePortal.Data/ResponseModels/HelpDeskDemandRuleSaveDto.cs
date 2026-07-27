using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HelpDeskDemandRuleSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public DateTime? createdDate{get;set;}
        public DateTime? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? companies{get;set;}
        public string? helpDeskCategories{get;set;}
        public int? helpDeskRoleId{get;set;}
    }
}
