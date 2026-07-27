using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class SureliIsTakipSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? isinTanimi{get;set;}
        public int? companyId{get;set;}
        public string? baslamaTarihi{get;set;}
        public string? takipSorumlusu{get;set;}
        public int? mailSuresi{get;set;}
        public string? terminTarihi{get;set;}
        public bool? surekliMi{get;set;}
        public string? muhattaplar{get;set;}
        public bool? tamamlandi{get;set;}
        public string? files{get;set;}
        public string? aciklama{get;set;}
    }
}
