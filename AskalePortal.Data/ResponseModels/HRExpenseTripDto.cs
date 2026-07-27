using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HRExpenseTripDto
    {
        public int? id{get;set;}
        public int? userId{get;set;}
        public string? kisi{get;set;}
        public string? whereareyou{get;set;}
        public string? destination{get;set;}
        public string? gidisTarihi{get;set;}
        public string? donusTarihi{get;set;}
        public string? description{get;set;}
    }
}
