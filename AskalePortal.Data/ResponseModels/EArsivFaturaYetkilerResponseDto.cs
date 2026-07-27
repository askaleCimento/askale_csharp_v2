using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class EArsivFaturaYetkilerResponseDto
    {
        public int? id { get; set; }

        public int? userId { get; set; }

        public string? userName { get; set; }

        public HashSet<string>? companyNames { get; set; }

        public HashSet<int>? companyIds { get; set; }
    }
}
