using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRExpenseTypeTableSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public int? updatedUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public string? expenseTypeName{get;set;}
        public bool? toplamaNo{get;set;}
        public bool? harcamaBoyu{get;set;}
        public bool? otoparkMi{get;set;}
        public string? sapSide{get;set;}
    }
}
