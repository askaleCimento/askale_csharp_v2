using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class CustomerComplaintSaveDto
    {
      public int? id{get;set;}
      public bool? enabled{get;set;}
      public int? createdUserId{get;set;}
      public string? createdDate{get;set;}
      public string? updateDate{get;set;}
      public int? updatedUserId{get;set;}
      public string? title{get;set;}
      public string? musteriKodu{get;set;}
      public string? musteriAdi{get;set;}
      public string? malzemeTuru{get;set;}
      public double? malzemeMiktari{get;set;}
      public string? musteriTemsilcisi{get;set;}
      public string? musteriTel{get;set;}
      public string? musteriEmail{get;set;}
      public string? description{get;set;}
      public int? userId{get;set;}
      public int? companyId{get;set;}
      public int? categoryId{get;set;}
      public int? sikayetTipiId{get;set;}
    }
}
