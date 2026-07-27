using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class RatingQuestionDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? title{get;set;}
        public int? puanGorusGosterimi{get;set;}
        public int? count{get;set;}
        public double? average{get;set;}
        public bool? approval{get;set;}
        public int? ratingId{get;set;}
    }
}
