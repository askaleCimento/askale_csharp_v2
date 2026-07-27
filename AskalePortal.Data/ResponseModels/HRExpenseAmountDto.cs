using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRExpenseAmountDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public decimal? harcirahMiktari{get;set;}
        public string? calisanTuru{get;set;}
        public string? harcamaTuru{get;set;}
    }
}
