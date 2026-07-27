using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRExpenseWithOutTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
        public int? islemTuruId{get;set;}
        public int? expenseTypeId{get;set;}
        public decimal? totalLimitAmount{get;set;}
        public int? currentUserId{get;set;}
        public int? currentStateId{get;set;}
        public int? vekaletId{get;set;}
        public string? islemTarihi{get;set;}
        public string? tripDesciption{get;set;}
        public string? plaka{get;set;}
        public int? aracTuruId{get;set;}
        public decimal? amount{get;set;}
        public decimal? approvedAmount{get;set;}
        public int? kalinanGunSayisi{get;set;}
        public int? otoparkGunSayisi{get;set;}
        public string? kdvOrani{get;set;}
        public decimal? kdvDegeri{get;set;}
        public string? spendingTime{get;set;}
        public string? fileNames{get;set;}
        public bool? approval{get;set;}
        public bool? lastApproved{get;set;}
        public int? onaySirasi{get;set;}
        public bool? gunlukMu{get;set;}
        public string? hrNot{get;set;}
        public int? tripId{get;set;}
    }
}
