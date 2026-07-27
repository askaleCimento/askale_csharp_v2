using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class UserTelephoneTableDto
    {
        public int? id{get;set;}
        public int? userId{get;set;}
        public string? name{get;set;}
        public string? shortDescription{get;set;}
        public string? companyName{get;set;}
        public string? factoryNumber{get;set;}
        public string? factoryInternal{get;set;}
        public string? phoneNumber{get;set;}
        public string? shortCode{get;set;}
    }
}
