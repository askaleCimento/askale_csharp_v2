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
        public class EnerjiPiyasasiVeriTable : BaseBLL<AskalePortal.Data.Models.EnerjiPiyasasiVeriTable>
        {
            public EnerjiPiyasasiVeriTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.EnerjiPiyasasiVeriTable> GetByDateAndCompanyId(DateTime date, int companyID, int bolumId)
            {
                return dal.Get(u => u.enabled == true && u.date == date.Date && u.companyId == companyID && u.bolumId==bolumId).ToList();
            }

            public List<AskalePortal.Data.Models.EnerjiPiyasasiVeriTable> GetByDateAndCompanyId(DateTime date, int companyID)
            {
                return dal.Get(u => u.enabled == true && u.date == date.Date && u.companyId == companyID).ToList();
            }
        }
    }

    
}
