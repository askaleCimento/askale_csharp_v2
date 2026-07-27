using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class IncomingDocumentSourceSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? title{get;set;}
        public string? subTitle{get;set;}
        public string? subject{get;set;}
        public string? phone{get;set;}
        public string? fax{get;set;}
    }
}
