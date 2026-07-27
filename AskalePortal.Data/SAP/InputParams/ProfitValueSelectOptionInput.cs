using AskalePortal.Data.SAP.Models;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{




    public class ProfitValueSelectOptionInput
    {
        [SapName("LV_BUKRS")]
       public SelectOption[]? selectOptions {  get; set; }

        [SapName("LV_PERBL")]
        public string? date {  get; set; }
    }
}
