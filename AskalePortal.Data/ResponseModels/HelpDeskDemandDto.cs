using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HelpDeskDemandDto
    {
        public int? Id{get;set;}
        public string? talepNo{get;set;}
        public string? sirket{get;set;}
        public string? kullanici{get;set;}
        public string? talep{get;set;}
        public string? durum{get;set;}
        public string? kategori{get;set;}
        public string? oncelik{get;set;}
        public string? atanan{get;set;}
        public DateTime tarih{get;set;}
    }
}
