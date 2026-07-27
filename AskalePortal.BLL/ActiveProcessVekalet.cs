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
        public class ActiveProcessVekalet : BaseBLL<AskalePortal.Data.Models.ActiveProcessVekalet>
        {
            public ActiveProcessVekalet(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public AskalePortal.Data.Models.ActiveProcessVekalet? GetByAlanUserId(int vekaletAlanId)
            {
                return dal.Get(u => u.VekaletAlanId == vekaletAlanId && u.enabled == true).FirstOrDefault();
            }

           
        }
    }
}
