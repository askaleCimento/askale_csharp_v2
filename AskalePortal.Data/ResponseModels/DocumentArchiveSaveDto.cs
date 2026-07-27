using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class DocumentArchiveSaveDto
    {
       public int? id{get;set;}
       public string? title{get;set;}
       public bool? isTemplate{get;set;}
       public string? createdDate{get;set;}
       public string? updateDate{get;set;}
       public int? createdUserId{get;set;}
       public int? updatedUserId{get;set;}
        public bool? enabled{get;set;}
    }
}
