using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class IcYazismalarTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public int? companyId{get;set;}
        public string? servisi{get;set;}
        public string? konu{get;set;}
        public string? kanalGorusu{get;set;}
        public string? tarih{get;set;}
        public int? kanalId{get;set;}
        public string? icerik{get;set;}
        public int? onaylayici1{get;set;}
        public int? onaylayici2{get;set;}
        public int? onaylayici3{get;set;}
        public int? onaylayici4{get;set;}
        public int? birimAmiriId{get;set;}
        public bool? onay1Ok{get;set;}
        public bool? onay2Ok{get;set;}
        public bool? onay3Ok{get;set;}
        public bool? onay4Ok{get;set;}
        public bool? onaylandiMi{get;set;}
        public string? disaprovecondition{get;set;}
        public bool? redEttiMi{get;set;}
        public bool? bittiMi{get;set;}
    }
}
