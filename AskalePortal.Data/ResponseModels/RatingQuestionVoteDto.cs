using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class RatingQuestionVoteDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public int? ratingValue{get;set;}
        public string? comment{get;set;}
        public int? userId{get;set;}
        public int? questionId{get;set;}
        public int? ratingId{get;set;}
    }
}
