using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ISGAksiyonTableSaveDto
    {
       public int? id{get;set;}
       public bool? enabled{get;set;}
       public int? createdUserId{get;set;}
       public string? createdDate{get;set;}
       public string? updateDate{get;set;}
       public int? updatedUserId{get;set;}
       public string? uygunsuzlukTarihi{get;set;}
       public int? bidirimdeBulunan{get;set;}
       public string? uygunsuzlukAciklama{get;set;}
       public string? uygunsuzlukOneri{get;set;}
       public bool? bittiMi{get;set;}
       public int? companyId{get;set;}
       public int? uygunsuzlukKaynagiId{get;set;}
       public int? uygunsuzlukBulunanUniteId{get;set;}
       public string? fileOnceki{get;set;}
       public string? fileSonraki{get;set;}
    }
}
