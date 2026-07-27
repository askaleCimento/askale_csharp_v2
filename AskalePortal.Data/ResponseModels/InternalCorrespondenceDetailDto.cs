using AskalePortal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class InternalCorrespondenceDetailDto
    {
        public int? id{get;set;}
        public string? companyName{get;set;}
        public string? companyTitle{get;set;}
        public string? companyLongName{get;set;}
        public string? servisi{get;set;}
        public string? konu{get;set;}
        public DateTime? createdDate{get;set;}
        public string? kanal{get;set;}
        public string? createdUser{get;set;}
        public string? icerik{get;set;}
        public string? note{get;set;}
        public string? noteUserName{get;set;}
        public List<AttachedFile>? listAttachedFile{get;set;}
        public List<OnaylayiciDto>? listOnayDurumu{get;set;}
        public List<InternalCorrespondenceMessageDto>? listInternalCorrespondenceMessageDtos{get;set;}

    }
}
