using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class InternalCorrespondenceMessageDto
    {
        public int? id{get;set;}
        public string? time{get;set;}
        public string? username{get;set;}
        public string? message{get;set;}
    }
}
