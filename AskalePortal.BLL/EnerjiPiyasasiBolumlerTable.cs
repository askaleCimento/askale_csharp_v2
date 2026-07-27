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
        public class EnerjiPiyasasiBolumlerTable : BaseBLL<AskalePortal.Data.Models.EnerjiPiyasasiBolumlerTable>
        {
            public EnerjiPiyasasiBolumlerTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.EnerjiPiyasasiBolumlerTable> GetByCompanyId(int companyID)
            {
                return dal.Get(u => u.enabled == true  && u.companyId == companyID).ToList();
            }
        }
    }

    
}
