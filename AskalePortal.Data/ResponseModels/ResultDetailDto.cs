using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ResultDetailDto
    {
        public string? name { get; set; }
        public string? companyName{ get; set; }
        public List<RatingAnswers>? listAnswers{ get; set; }
    }

    public class RatingAnswers
    {
        public string? gorus{ get; set; }

        public int? puan{ get; set; }
    }
}
