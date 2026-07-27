using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class SureliIslerTakipDtoParameter
    {
        public int? userId { get; set; }
        public int? filterUserId { get; set; }
        public int? filterCompanyId { get; set; }
        public string? filterAciklama { get; set; }

    }
}
