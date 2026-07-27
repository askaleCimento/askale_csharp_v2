using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class EArsivFaturaResponseDto
    {
       public string? ettn{get;set;}
       public string? belgeNumarasi{get;set;}
       public string? saticiVknTckn{get;set;}
       public string? saticiUnvanAdSoyad{get;set;}
       public string? belgeTarihi{get;set;}
       public string? belgeTuru{get;set;}
       public string? onayDurumu{get;set;}
       public int? companyId{get;set;}
       public string? username{get;set;}
       public string? companyName{get;set;}
       public bool? bittiMi{get;set;}
    }
}
