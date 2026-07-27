using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class RoleDetailSaveDto
    {
       public int? id{get;set;}
       public bool? enabled{get;set;}
       public int? moduleId{get;set;}
       public bool? canSee{get;set;}
       public bool? canAdd{get;set;}
       public bool? canEdit{get;set;}
       public bool? canDelete{get;set;}
       public bool? canApprove{get;set;}
       public bool? canSeeLogs{get;set;}
       public int? roleId{get;set;}
       public string? createdDate{get;set;}
       public string? updateDate{get;set;}
       public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
    }
}
