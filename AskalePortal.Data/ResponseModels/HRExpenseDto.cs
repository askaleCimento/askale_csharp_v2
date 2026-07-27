using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRExpenseDto
    {
        public int? id{get;set;}
        public string? file{get;set;}
        public string? harcamaTuru{get;set;}
        public string? harcamaTarihi{get;set;}
        public int? gunSayisi{get;set;}
        public decimal? toplamLimit{get;set;}
        public decimal? harcamaTutari{get;set;}
        public decimal? onaylananMasraf{get;set;}
        public string? aciklama{get;set;}
        public int? currentStateId{get;set;}
        public int? currentUserId{get;set;}
        public bool? approval{get;set;}
        public int? onaySirasi{get;set;}
    }
}
