using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HREmployeeType : BaseBLL<AskalePortal.Data.Models.HREmployeeType>
        {
            public HREmployeeType(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public int GetByName(string calisanTuru)
            {
                return dal.Get(u => u.calisanTuru == calisanTuru.Trim() && u.enabled == true).Count();
            }
            public int GetByNameClass(AskalePortal.Data.Models.HREmployeeType entity)
            {
                return dal.Get(u => u.calisanTuru == entity.calisanTuru.Trim() && u.Id!=entity.Id && u.enabled == true).Count();
            }
        }
    }
}
