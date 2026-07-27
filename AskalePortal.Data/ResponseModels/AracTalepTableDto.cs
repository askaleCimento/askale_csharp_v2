using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class AracTalepTableDto
    {
        public int? id{get;set;}
        public string? baslangicTarihi{get;set;}
        public string? teslimTarihi{get;set;}
        public string? destinationLocation{get;set;}
        public string? aciklama{get;set;}
        public string? createdUser{get;set;}
        public int? createdUserId{get;set;}
        public int? onaySirasi{get;set;}
        public string? plaka{get;set;}
    }
}
