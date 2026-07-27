using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HelpDeskDemandSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? ticketNumber{get;set;}
        public string? createdByUserName{get;set;}
        public string? internalNumber{get;set;}
        public string? teamviewerId{get;set;}
        public string? teamviewerPassword{get;set;}
        public string? title{get;set;}
        public string? description{get;set;}
        public string? timeSpent{get;set;}
        public bool? isClosed{get;set;}
        public int? helpDeskTypeId{get;set;}
        public int? helpDeskStatusId{get;set;}
        public int? helpDeskCategoryId{get;set;}
        public int? createdByCompanyId{get;set;}
        public int? assignedToHelpDeskRoleId{get;set;}
    }
}
