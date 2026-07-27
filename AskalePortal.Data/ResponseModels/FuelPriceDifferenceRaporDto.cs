using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class FuelPriceDifferenceRaporDto
    {
        public DateTime? tarih{get;set;}
        public decimal? eskiMotorin{get;set;}
        public decimal? yeniMotorin{get;set;}
        public decimal? kdvDahil{get;set;}
        public decimal? kdvHaric{get;set;}
        public string? companyName{get;set;}
        public List<FuelPriceDifferenceModelDto>? liste{get;set;}
        public List<FuelDifferenceApproverDto>? listOnaylayici{get;set;}
    }

    public class FuelPriceDifferenceModelDto
    {

        public string? yukleniciFirma{get;set;}
        public string? isinAdi{get;set;}
        public string? nevi{get;set;}
        public decimal? km{get;set;}
        public decimal? eskiFiyat{get;set;}
        public decimal? yenifiyat{get;set;}
        public decimal? katsayi{get;set;}

    }
    public class FuelDifferenceApproverDto
    {

        public string? name{get;set;}
        public int? siraNo{get;set;}
    }


}

