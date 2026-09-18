using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ActiveProcessChecksSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? belnr{get;set;}
        public string? kunnr{get;set;}
        public string? name1{get;set;}
        public string? netdt{get;set;}
        public double? wrbtr{get;set;}
        public int? activeProcessId{get;set;}
    }
}
