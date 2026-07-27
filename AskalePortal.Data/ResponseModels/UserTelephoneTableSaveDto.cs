using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class UserTelephoneTableSaveDto
    {
        public int? id{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
        public DateTime? createdDate{get;set;}
        public DateTime? updateDate{get;set;}
        public string? factoryNumber{get;set;}
        public string? factoryInternal{get;set;}
        public string? phoneNumber{get;set;}
        public string? shortCode{get;set;}
        public int? userId{get;set;}
        public bool? enabled { get; set; }
        public bool? kvkkOnay{get;set;}
    }
}
