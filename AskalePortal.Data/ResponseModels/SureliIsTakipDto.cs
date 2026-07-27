using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class SureliIsTakipDto
    {
        public int? id{get;set;}
        public string? fabrika{get;set;}
        public string? isinTanimi{get;set;}
        public DateTime? baslamaTarihi{get;set;}
        public DateTime? terminTarihi{get;set;}
        public int? mailSuresi{get;set;}
        public string? takipSorumlusu{get;set;}
        public string? ilgililer{get;set;}
        public bool? tamamlandimi{get;set;}
        public string? aciklama{get;set;}
        public string? fileNames{get;set;}
        public string? olusturanKisi{get;set;}
        public int? olusturanKisiId{get;set;}
    }
}
