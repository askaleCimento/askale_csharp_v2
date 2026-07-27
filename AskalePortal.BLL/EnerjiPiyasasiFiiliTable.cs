using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class EnerjiPiyasasiFiiliTable : BaseBLL<AskalePortal.Data.Models.EnerjiPiyasasiFiiliTable>
        {
            public EnerjiPiyasasiFiiliTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.EnerjiPiyasasiFiiliTable GetByDateAndCompanyId(DateTime date, int companyID)
            {
                return dal.Get(u => u.enabled == true && u.date == date.Date && u.companyId == companyID).FirstOrDefault() ?? new AskalePortal.Data.Models.EnerjiPiyasasiFiiliTable();
            }
        }
    }

    
}
