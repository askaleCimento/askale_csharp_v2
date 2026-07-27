using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ApprovedPerson
    {
        public int? userId{get;set;}
        public string? dateTime{get;set;}
        public bool? process{get;set;}
        public string? companyName{get;set;}
    }
}
