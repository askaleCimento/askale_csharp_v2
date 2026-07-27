using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
		public class IsletmeToplantisiGidenMailTable : BaseBLL<AskalePortal.Data.Models.IsletmeToplantisiGidenMailTable>
		{
			
            public IsletmeToplantisiGidenMailTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }


            public List<AskalePortal.Data.Models.IsletmeToplantisiGidenMailTable> GetByFabrika(int companyId)
            {
				var q = dal.Get(u => u.companyId == companyId).ToList();
				return q;
			}

			public List<AskalePortal.Data.Models.IsletmeToplantisiGidenMailTable> GetAllWithPage(int activePage, int recordsPerPage, AdminUser aDMIN_USER)
			{
				if (aDMIN_USER.roleId == 1)
				{
					return dal.Get(u => u.enabled == true).OrderByDescending(u=>u.Id).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                }
				else
				{
					return dal.Get(u => u.companyId == aDMIN_USER.companyId && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                }
			
			}
		}
	}

}
