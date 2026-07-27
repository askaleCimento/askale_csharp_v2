using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class IcYazismaTableDto
    {
        public int? id{get;set;}
        public string? companyName{get;set;}
        public string? servisi{get;set;}
        public string? konu{get;set;}
        public DateTime? createdDate{get;set;}
        public string? kanal{get;set;}
        public string? createdUser{get;set;}
        public bool? status{get;set;}
        public int? createdUserId{get;set;}
        public bool? onay1Ok{get;set;}
    }
}
