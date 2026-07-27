using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class AnnualLeaveDetailDto
    {
       public int? id{get;set;}
       public string? sicilNo{get;set;}
       public string? name{get;set;}
       public string? iseGirisTarihi{get;set;}
       public string? departman{get;set;}
       public string? pozisyon{get;set;}
       public int? izinTuru{get;set;}
       public string? typeName{get;set;}
       public string? typeNameEn{get;set;}
       public string? digerAciklama{get;set;}
       public string? mevcutIzin{get;set;}
       public string? istenenIzin{get;set;}
       public string? startdate{get;set;}
       public string? endDate{get;set;}
       public string? kalanIzin{get;set;}
       public string? adres{get;set;}
       public string? vekaletName{get;set;}
       public string? birinciOnayTarihi{get;set;}
       public string? birinciOnaylayici{get;set;}
       public int? birinciDurum{get;set;}
       public List<int>? birinciOnaylayiciFile{get;set;}
       public string? ikinciOnayTarihi{get;set;}
       public string? ikinciOnaylayici{get;set;}
       public int? ikinciDurum{get;set;}
       public List<int>? ikinciOnaylayiciFile{get;set;}
       public string? ucuncuOnayTarihi{get;set;}
       public string? ucuncuOnaylayici{get;set;}
       public int? ucuncuDurum{get;set;}
       public List<int>? ucuncuOnaylayiciFile{get;set;}
       public string? dorduncuOnayTarihi{get;set;}
       public string? dorduncuOnaylayici{get;set;}
       public int? dorduncuDurum{get;set;}
       public List<int>? dorduncuOnaylayiciFile{get;set;}
       public string? besinciOnayTarihi{get;set;}
       public string? besinciOnaylayici{get;set;}
       public int? besinciDurum{get;set;}
       public List<int>? besinciOnaylayiciFile{get;set;}
       public string? altinciOnayTarihi{get;set;}
       public string? altinciOnaylayici{get;set;}
       public int? altinciDurum{get;set;}
       public List<int>? altinciOnaylayiciFile{get;set;}
    }
}
