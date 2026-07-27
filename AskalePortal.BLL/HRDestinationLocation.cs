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
        public class HRDestinationLocation : BaseBLL<AskalePortal.Data.Models.HRDestinationLocationTable>
        {
            public HRDestinationLocation(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public int GetByName(string destinationLocation)
            {
                return dal.Get(u => u.destinationLocation == destinationLocation.Trim() && u.enabled == true).Count();
            }
            public int GetByNameClass(AskalePortal.Data.Models.HRDestinationLocationTable entity)
            {
                return dal.Get(u => u.destinationLocation == entity.destinationLocation.Trim() && u.Id!=entity.Id && u.enabled == true).Count();
            }
        }
    }
}
