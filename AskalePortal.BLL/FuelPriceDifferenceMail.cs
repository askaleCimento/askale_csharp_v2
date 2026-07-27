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
        public class FuelPriceDifferenceMail : BaseBLL<AskalePortal.Data.Models.FuelPriceDifferenceMail>
        {
            public FuelPriceDifferenceMail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }

            public List<Data.Models.FuelPriceDifferenceMail> listAllByEnabled(bool v)
            {
                return dal.Get(u => u.enabled).ToList();
            }
        }
    }
}
