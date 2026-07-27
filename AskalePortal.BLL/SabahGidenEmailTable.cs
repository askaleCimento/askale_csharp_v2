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
		public class SabahGidenEmailTable : BaseBLL<AskalePortal.Data.Models.SabahGidenEmailTable>
		{
            public SabahGidenEmailTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.SabahGidenEmailTable> GetAllWithPage(int activePage, int pageSize, AdminUser aDMIN_USER)
			{

				if (aDMIN_USER.roleId == 1)
				{
					return dal.Get(u => u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                }
				else
				{
					return dal.Get(u => u.companyId == aDMIN_USER.companyId && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                }

				
			}
		}
	}

}
