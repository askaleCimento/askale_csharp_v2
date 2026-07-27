using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ExecutiveDocumentsDto
    {
        public int? id{get;set;}
        public string? documentId{get;set;}
        public int? topID{get;set;}
        public int? typeID{get;set;}
        public string? typeName{get;set;}
        public string? title{get;set;}
        public string? filename{get;set;}
        public int? fileSize{get;set;}
        public DateTime? createdDate{get;set;}
    }
}
