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
		public class HRAccountJobTable : BaseBLL<AskalePortal.Data.Models.HRAccountJobTable>
		{
            public HRAccountJobTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.HRAccountJobTable> GetByUnsent()
            {
				return dal.Get(u => u.isSent == false && u.enabled == true).ToList();
            }
		}
	}

}
