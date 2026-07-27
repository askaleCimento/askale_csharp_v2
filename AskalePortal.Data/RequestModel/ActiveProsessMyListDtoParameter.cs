using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class ActiveProsessMyListDtoParameter
    {
        public string? relatedDataId{get;set;}
        public string? relatedDataDesc{get;set;}
        public string? relatedDataPrimaryId{get;set;}
        public string? relatedDataPrimaryDesc{get;set;}
        public int? stateId{get;set;}
        public int? userId{get;set;}
        public string? type { get; set; }
    }
}
