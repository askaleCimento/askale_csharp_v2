using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class PressAnnouncementDto
    {
        public int? id{get;set;}
        public string? title{get;set;}
        public string? description{get;set;}
        public string? imageUrl{get;set;}
        public string? createdByUserName{get;set;}
        public DateTime? newsDate{get;set;}
    }
}
