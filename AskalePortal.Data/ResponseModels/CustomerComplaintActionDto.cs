using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.Data.ResponseModels
{
    public class CustomerComplaintActionDto
    {
        public int? id{get;set;}
        public int? sikayetId{get;set;}
        public string? actionType{get;set;}
        public string? companyName{get;set;}
        public string? olusturanKisi{get;set;}
        public DateTime? olusturmaTarihi{get;set;}
        public bool? enabled{get;set;}
        public List<string>? fileNames{get;set;}
        public string? actionDescription{get;set;}
    }
}
