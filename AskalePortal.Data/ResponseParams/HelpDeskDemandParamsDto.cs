using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseParams
{
    public class HelpDeskDemandParamsDto
    {
        public string? filterbaslik{get;set;}
        public int? filterCompanyId{get;set;}
        public int? filterCategoryId{get;set;}
        public int? filterTypeId{get;set;}
        public int? filterStatusId{get;set;}
        public int? filterRoleId{get;set;}
        public int? filterUserId{get;set;}
    }
}
