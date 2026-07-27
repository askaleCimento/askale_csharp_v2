using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.Data.ResponseModels
{
   public class ComingDocumentTypeDto
    {
        public int? id{get;set;}
        public string? title{get;set;}
        public DateTime? olusturmaTarihi{get;set;}
        public bool? enabled{get;set;}
        public string? olusturanKisi{get;set;}
    }
}
