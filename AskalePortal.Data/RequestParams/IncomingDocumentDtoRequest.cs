using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestParams
{
    public class IncomingDocumentDtoRequest
    {
        public int? userId{get;set;}
        public int? sourceId{get;set;}
        public bool? isOutgoing{get;set;}
        public DateTime? girisTarihi{get;set;}
        public string? title{get;set;}
        public int? documentOrder{get;set;}

    }
}
