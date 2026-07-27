using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class SozlesmeTableDto
    {
        public int? id{get;set;}
        public string? company{get;set;}
        public string? sozlesmeCinsi{get;set;}
        public string? firmaKodu{get;set;}
        public string? firmaAdi{get;set;}
        public string? sozlesmeKonusu{get;set;}
        public string? aciklama{get;set;}
        public double? tutar{get;set;}
        public DateTime? bitisTarihi{get;set;}
        public string? paraBirimi{get;set;}
        public List<string>? picture{get;set;}
        public bool? tamamMi{get;set;}
    }
}
