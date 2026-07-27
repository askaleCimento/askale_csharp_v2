using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.Data.ResponseModels
{
    public class SozlesmeTablePdfDto
    {
        public int? id;
        public string? vkorg;
        public string? vtext;
        public string? satinAlmaGrubu;
        public string? firmaKodu;
        public string? iletisim;
        public string? firmaYetkisi;
        public string? aciklama;
        public decimal? sozlesmeTutari;
        public string? sozlesmeTutarBirimi;
        public int? sozlesmeOdemeVadesi;
        public decimal? odemeAvansYuzdesi;
        public decimal? odemeAvansTutari;
        public string? odemeAvansBirimi;
        public string? damgaVergisiOdemesi;
        public DateTime? imzalananTarih;
        public DateTime? baslangicTarihi;
        public DateTime? bitisTarihi;
        public DateTime? uyariTarihi;
        public bool? teminatVarmi;
        public DateTime? teminatBaslangic;
        public DateTime? teminatBitis;
        public decimal? teminatTutari;
        public string? teminatTutariBirimi;
        public bool? tamammi;
    }
}
