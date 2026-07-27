using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class SozlesmeTableListDtoParameter
    {
        public int? filterSozlesmeNo { get; set; }
        public int? filterCompanyId { get; set; }
        public int? filterSozlesmeCinsiId { get; set; }
        public string? filterSozlesmeKonusu { get; set; }
        public string? filterAciklama { get; set; }
        public string? filterSozlesmeTutari { get;set; }
        public DateTime? filterBaslangicTarih { get; set; }
        public DateTime? filterBitisTarih { get; set; }
        public bool? filterTamamlandimi { get; set; }
        public string? filterFirmaAdi { get; set; }
    }
}
