using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.Models
{
    public partial class ActiveProcessInvoice
    {
        public int? id{get;set;}
        public DateTime? createdDate{get;set;}
        public DateTime? updateDate{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
        public bool? enabled{get;set;}
        public string? belnr{get;set;}
        public string? bukrs{get;set;}
        public int? gjahr{get;set;}
        public string? dagitimkanali{get;set;}
        public double? dmshb{get;set;}
        public string? bldat{get;set;}
        public string? faedt{get;set;}
        public string? zterm{get;set;}
        public int? activeProcessId{get;set;}
    }
}
