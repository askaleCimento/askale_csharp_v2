using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class IncomingDocumentMyEditDto
    {
        public int? id{get;set;}
        public string? createdUserName{get;set;}
        public string? createdCompanyName{get;set;}
        public string? userName{get;set;}
        public string? userCompanyName{get;set;}
        public DateTime? incomingDate{get;set;}
        public string? typeTitle{get;set;}
        public string? sourceTitle{get;set;}
        public string? title{get;set;}
        public DateTime? documentDate{get;set;}
        public string? documentSpecialNumber{get;set;}
        public string? notes{get;set;}
        public bool? isCompleted{get;set;}
        public List<string>? files{get;set;}
    }
}
