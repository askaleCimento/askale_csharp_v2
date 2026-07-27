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
		public class HRVergiDilimleriTable : BaseBLL<AskalePortal.Data.Models.HRVergiDilimleriTable>
		{
            public HRVergiDilimleriTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.HRVergiDilimleriTable GetByYear(int ilkYil)
			{
				return dal.Get(u => u.yil == ilkYil && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.HRVergiDilimleriTable();
			}
		}
	}
}
