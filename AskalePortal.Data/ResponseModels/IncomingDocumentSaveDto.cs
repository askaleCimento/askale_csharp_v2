using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class IncomingDocumentSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? documentNumber{get;set;}
        public bool? isOutgoing{get;set;}
        public int? documentOrder{get;set;}
        public string? userIds{get;set;}
        public string? incomingDate{get;set;}
        public bool? hasAttachment{get;set;}
        public string? title{get;set;}
        public string? notes{get;set;}
        public string? documentDate{get;set;}
        public string? documentSpecialNumber{get;set;}
        public string? due{get;set;}
        public string? dueDate{get;set;}
        public bool? isRead{get;set;}
        public string? readDate{get;set;}
        public bool? isCompleted{get;set;}
        public string? completedDate{get;set;}
        public int? userId{get;set;}
        public int? companyId{get;set;}
        public int? sourceId{get;set;}
        public int? typeId{get;set;}
    }
}
