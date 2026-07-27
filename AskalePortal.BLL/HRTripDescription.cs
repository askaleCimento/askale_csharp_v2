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
        public class HRTripDescription : BaseBLL<AskalePortal.Data.Models.HRTripDescription>
        {
            public HRTripDescription(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public int GetByName(string tripDescription)
            {
               return dal.Get(u => u.tripDescription == tripDescription && u.enabled == true).Count();
            }

            public int GetByNameClass(AskalePortal.Data.Models.HRTripDescription entity)
            {
                return dal.Get(u => u.tripDescription == entity.tripDescription.Trim() && u.Id != entity.Id && u.enabled == true).Count();
            }
        }
    }
}
