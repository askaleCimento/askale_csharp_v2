using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseParams
{
    public class UsersFilterDto
    {
       public int? id{get;set;}
       public string? role{get;set;}
       public int? roleId{get;set;}
       public int? companyId{get;set;}
       public string? name{get;set;}
       public string? userName{get;set;}
       public string? email{get;set;}
       public string? vkorg{get;set;}
       public string? sapUserName{get;set;}
       public bool? approval{get;set;}
    }
}
