using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
		public class SozlesmeUpdateSatici : BaseBLL<AskalePortal.Data.Models.SozlesmeUpdateSatici>
		{
            public SozlesmeUpdateSatici(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public AskalePortal.Data.Models.SozlesmeUpdateSatici GetBySirketID(int sirket)
			{
				return dal.Get(u => u.companyId == sirket).FirstOrDefault() ?? new AskalePortal.Data.Models.SozlesmeUpdateSatici();
			}
		}
	}
}