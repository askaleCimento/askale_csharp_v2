using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HrUserDto
    {
        public int? id{get;set;}
        public string? name{get;set;}
        public string? username{get;set;}
        public string? departmanAdi{get;set;}
        public string? perNo{get;set;}
        public string? kullaniciTuru{get;set;}
        public string? hrEmployer1name{get;set;}
        public bool? hrEmployer1change{get;set;}
        public string? manager1{get;set;}
        public bool? manager1change{get;set;}
        public string? manager2{get;set;}
        public bool? manager2change{get;set;}
        public string? manager3{get;set;}
        public bool? manager3change{get;set;}
        public string? manager4{get;set;}
        public bool? manager4change{get;set;}
        public string? company{get;set;}
    }
}
