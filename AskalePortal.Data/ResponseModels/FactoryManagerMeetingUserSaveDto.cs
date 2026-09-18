using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class FactoryManagerMeetingUserSaveDto
    {
       public int? id{get;set;}
       public bool? enabled{get;set;}
       public int? createdUserId{get;set;}
       public string? createdDate{get;set;}
       public string? updateDate{get;set;}
       public int? updatedUserId{get;set;}
       public int? dataOrder{get;set;}
       public string? name{get;set;}
       public string? title{get;set;}
       public string? email{get;set;}
       public string? mobile{get;set;}
    }
}
