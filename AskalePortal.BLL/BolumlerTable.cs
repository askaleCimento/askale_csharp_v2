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
		public class BolumlerTable : BaseBLL<AskalePortal.Data.Models.BolumlerTable>
		{
            public BolumlerTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.BolumlerTable> GetByFabrika(int vKORG)
			{
				var q = dal.Get(u => u.sirketId == vKORG).ToList();
				return q;
			}
		}
	}

}
