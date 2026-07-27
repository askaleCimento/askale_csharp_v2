using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class MeetingDetailSaveDto
    {
        public int? id{get;set;}
        public bool? enabled{get;set;}
        public int? createdUserId{get;set;}
        public string? createdDate{get;set;}
        public string? updateDate{get;set;}
        public int? updatedUserId{get;set;}
        public string? detailNumber{get;set;}
        public int? copyFromMeetingId{get;set;}
        public int? copyFromMeetingDetailId{get;set;}
        public string? meetingDate{get;set;}
        public string? title{get;set;}
        public string? description{get;set;}
        public string? users{get;set;}
        public string? plannedDate{get;set;}
        public string? completedDate{get;set;}
        public string? completedNote{get;set;}
        public int? itemStatus{get;set;}
        public int? meetingId{get;set;}
    }
}
