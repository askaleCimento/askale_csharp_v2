using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class CustomerComplaintListDtoParameter
    {

        public int? userId { get; set; }
        public string? customerName { get; set; }
        public string? customerCode { get; set; }
        public string? malzemeName { get; set; }
        public int? companyId { get; set; }
        public int? categoryId { get; set; }
        public int? sikayetId { get; set; }


    }
}
