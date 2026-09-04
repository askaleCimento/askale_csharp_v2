using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ISGAksiyonTakipTableSaveDto
    {
       public int? id{get;set;}
       public bool? enabled{get;set;}
       public int? createdUserId{get;set;}
       public string? createdDate{get;set;}
       public string? updateDate{get;set;}
       public int? updatedUserId{get;set;}
       public double? oncekiOlasilik{get;set;}
       public double? oncekiSiklik{get;set;}
       public double? oncekiSiddet{get;set;}
       public double? oncekiRisk{get;set;}
       public string? planlananTarih{get;set;}
       public string? gerceklesenTarih{get;set;}
       public string? kisaAciklama{get;set;}
       public string? alinmasiGerekenOnlemler{get;set;}
       public int? oncelik{get;set;}
       public int? sapEmriAc{get;set;}
       public int? aksiyonId{get;set;}
       public string? aksiyonSorumlulari{get;set;}
    }
}
