using AskalePortal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class IcYazismaResponseMyList
    {
        public IcYazismalarTableSaveDto? icYazismalarTable{get;set;}
        public string? kanalGorusuFirst{get;set;}
    }
}
