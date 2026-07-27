using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class AnnualTableFilterDtoRequest
    {
     
        public int? id { get; set; }
        public int? currentStateId { get; set; }
        public int? userId { get; set; }
        public int? searchUserId { get; set; }
    
    };
}

