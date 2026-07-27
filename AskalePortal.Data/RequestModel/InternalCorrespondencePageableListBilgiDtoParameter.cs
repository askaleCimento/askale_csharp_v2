using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class InternalCorrespondencePageableListBilgiDtoParameter
    {
        public int? id { get; set; }
        public int? companyId { get; set; }
        public string? servisi { get; set; }
        public string? konu { get; set; }
        public bool? bittiMi { get; set; }
        public bool? redEttiMi { get; set; }
        public int? userId { get; set; }
    }
}
