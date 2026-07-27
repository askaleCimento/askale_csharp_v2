using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class EArsivFaturaSaveDto
    {
        public string? ettn { get; set; }
        public string? belgeNumarasi { get; set; }
        public string? saticiVknTckn { get; set; }
        public string? saticiUnvanAdSoyad { get; set; }
        public string? belgeTarihi { get; set; }
        public string? belgeTuru { get; set; }
        public string? onayDurumu { get; set; }
        public int? companyId { get; set; }
        public int? userId { get; set; }
        public int? iptalItiraz { get; set; }
        public int? talepDurum { get; set; }
        public bool? bittiMi { get; set; }
        public bool? enabled { get; set; }
        public string? pullTime { get; set; }
    }
}
