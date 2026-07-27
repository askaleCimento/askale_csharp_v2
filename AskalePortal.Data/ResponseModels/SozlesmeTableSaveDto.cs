using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class SozlesmeTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? satinAlmaGrubu{get;set;}
        public string? firmaKodu{get;set;}
        public string? iletisim{get;set;}
        public string? firmaYetkilisi{get;set;}
        public string? sozlesmeKonusu{get;set;}
        public string? aciklama{get;set;}
        public double? sozlesmeTutari{get;set;}
        public int? sozlesmeOdemeVadesi{get;set;}
        public double? odemeAvansYuzdesi{get;set;}
        public double? odemeAvansTutari{get;set;}
        public string? damgaVergisiOdemesi{get;set;}
        public string? imzalananTarih{get;set;}
        public string? baslangicTarihi{get;set;}
        public string? bitisTarihi{get;set;}
        public string? uyariTarihi{get;set;}
        public string? bildirimYapilacakKisiler{get;set;}
        public bool? teminatVarmi{get;set;}
        public string? teminatBaslangic{get;set;}
        public string? teminatBitis{get;set;}
        public double? teminatTutari{get;set;}
        public bool? tamamMi{get;set;}
        public int? companyId{get;set;}
        public int? sozlesmeTutarBirimiId{get;set;}
        public int? odemeAvansBirimiId{get;set;}
        public int? teminatTutariParaBirimId{get;set;}
        public int? sozlesmeTuruId{get;set;}
    }
}
