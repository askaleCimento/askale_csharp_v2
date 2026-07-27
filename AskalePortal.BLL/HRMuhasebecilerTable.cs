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
		public class HRMuhasebecilerTable : BaseBLL<AskalePortal.Data.Models.HRMuhasebecilerTable>
		{
            public HRMuhasebecilerTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.AdminUser> GetMuhasebeci(int companyId)
            {
				List<AskalePortal.Data.Models.AdminUser> liste = new List<AskalePortal.Data.Models.AdminUser>();
				List<AskalePortal.Data.Models.HRMuhasebecilerTable> list= dal.Get(u => u.companyId == companyId && u.enabled == true).ToList();
                foreach (var item in list)
                {
					liste.Add(item.user);
                }
				return liste;
            }
		}
	}

}
