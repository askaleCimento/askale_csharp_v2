using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class DieselPriceDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public int? kdvRate{get;set;}
        public string? girisTarihi{get;set;}
        public double? fiyat{get;set;}
        public string? createdDate{get;set;}
        public int? createdUserId{get;set;}
        public int? companyId{get;set;}
        public int? currentUserId{get;set;}
        public int? currentStateId{get;set;}
        public bool? approval{get;set;}
        public int? onaySirasi{get;set;}
    }
}
