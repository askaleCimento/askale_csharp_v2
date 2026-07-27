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
		public class UniteTurleriTable : BaseBLL<AskalePortal.Data.Models.UniteTurleriTable>
		{
            public UniteTurleriTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.UniteTurleriTable> GetAll(int pageNumber, int pageSize)
			{

				var q = dal.Get(k => k.enabled == true);
				q = q.OrderByDescending(u => u.Id);
				return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
            }
		}
	}
}
