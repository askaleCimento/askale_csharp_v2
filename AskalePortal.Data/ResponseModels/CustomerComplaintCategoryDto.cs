using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.Data.ResponseModels
{
    public class CustomerComplaintCategoryDto
    {
       public int? id{get;set;}
       public string? categoryName{get;set;}
       public DateTime? olusturmaTarihi{get;set;}
       public bool? enabled{get;set;}
       public string? olusturanKisi{get;set;}
    }
}
