using AskalePortal.Data.Models;
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
        public class DieselPriceDetail : BaseBLL<AskalePortal.Data.Models.DieselPriceDetail>
        {
            public DieselPriceDetail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }

            public List<Data.Models.DieselPriceDetail> getByDieselId(bool enabled, int dieselId)
            {
                return dal.Get(u => u.enabled == enabled && u.dieselId == dieselId).ToList();
            }

            public Data.Models.DieselPriceDetail getByActive(int dieselId,int userId)
            {
                return dal.Get(u => u.enabled && u.dieselId == dieselId && u.userId == userId).First();
            }
        }
    }
}
