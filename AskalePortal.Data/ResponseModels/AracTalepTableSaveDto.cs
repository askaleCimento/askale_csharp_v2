using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class AracTalepTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public int? currentUserId{get;set;}
        public int? currentStateId{get;set;}
        public string? baslangicTarihi{get;set;}
        public string? teslimTarihi{get;set;}
        public int? destinationLocationId{get;set;}
        public string? aciklama{get;set;}
        public bool? approval{get;set;}
        public int? onaySirasi{get;set;}
        public string? plaka{get;set;}
    }
}
