using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class OnaylayiciDto
    {
        public string? userName{get;set;}
        public string? onayDurumu{get;set;}
        public int? durum{get;set;}
        public List<int>? file{get;set;}
    }
}
