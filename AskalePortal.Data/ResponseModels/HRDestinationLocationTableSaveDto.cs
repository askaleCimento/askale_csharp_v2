using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRDestinationLocationTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public string? destinationLocation{get;set;}
        public int? geziYeriId{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
    }
}
