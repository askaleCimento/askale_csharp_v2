using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRTripDescriptionSaveDto
    {
        public int? id{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
        public bool? enabled{get;set;}
        public int? sapId{get;set;}
        public string? tripDescription{get;set;}
    }
}
