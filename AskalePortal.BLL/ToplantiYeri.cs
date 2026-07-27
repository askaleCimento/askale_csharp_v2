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
		public class ToplantiYeri : BaseBLL<AskalePortal.Data.Models.ToplantiYeriTable>
		{
            public ToplantiYeri(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.ToplantiYeriTable> GetByFabrika(int companyId)
			{
				var q = dal.Get(u => u.companyId == companyId).ToList();
				return q;
			}
		}
	}
}
