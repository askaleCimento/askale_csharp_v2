using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class FuelPriceDifferenceDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public int? userId{get;set;}
        public int? currentUserId{get;set;}
        public int? currentStateId{get;set;}
        public bool? approval{get;set;}
        public int? onaySirasi{get;set;}
        public string? yukleniciFirma{get;set;}
        public string? isinAdi{get;set;}
        public string? sozlesmeBitis{get;set;}
        public string? sozlesmeBaslangic{get;set;}
        public string? nevi{get;set;}
        public double? katSayi{get;set;}
        public double? km{get;set;}
        public double? fiyat{get;set;}
        public string? fiyatTarih{get;set;}
        public int? companyId{get;set;}
        public int? birimId{get;set;}
        public int? editId{get;set;}
    }
}
