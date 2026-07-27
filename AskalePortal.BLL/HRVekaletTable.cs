using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HRVekaletTable : BaseBLL<AskalePortal.Data.Models.HRVekaletTable>
        {
            public HRVekaletTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.HRVekaletTable GetByAlanUserId(int createdUserId)
            {
                return dal.Get(u => u.vekaletVerenId == createdUserId && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.HRVekaletTable();
            }
        }
    }
}
