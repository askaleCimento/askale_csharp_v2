using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class FinansUserTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? companyId{get;set;}
        public int? userId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
    }
}
