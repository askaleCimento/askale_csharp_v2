using AskalePortal.Data.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class HelpDeskMessageDto
    {
        public int id{get;set;}
        public ResponseByteArray? imgPhoto{get;set;}
        public string? message{get;set;}
        public string? imageUrl{get;set;}
        public string? username{get;set;}
        public DateTime createdDate{get;set;}
        public List<FileNameAndTitle>? fileName{get;set;}
    }

    public class FileNameAndTitle
    {
        public string? filename{get;set;}
        public string? title{get;set;}
    }

}
