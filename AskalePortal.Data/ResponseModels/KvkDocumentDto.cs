using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class KvkDocumentDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public int? archiveId{get;set;}
        public string? documentId{get;set;}
        public int? topId{get;set;}
        public int? typeId{get;set;}
        public string? typeName{get;set;}
        public string? title{get;set;}
        public string? fileName{get;set;}
        public int? fileSize{get;set;}
        public string? createdByUserName{get;set;}
    }
}
